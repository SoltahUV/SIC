using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ImageMagick;
using SIC.DTOs;
using SIC.Enums;
using SIC.Services.Interfaces;
using SIC.Helpers;

namespace SIC.Services;

public class ConversionService : IConversionService
{
    private readonly ILogger<ConversionService> _logger;

    public ConversionService(ILogger<ConversionService> logger)
    {
        _logger = logger;
    }

    public Task<ConversionResult> ConvertAsync(
        string sourcePath,
        ConversionOptions options,
        CancellationToken token)
    {
        return Task.Run(() =>
        {
            try
            {
                token.ThrowIfCancellationRequested();

                // Проверяем папку назначения
                _logger.LogDebug(
                    "Checking target directory {TargetPath}",
                    options.TargetPath);

                if (!Directory.Exists(options.TargetPath))
                {
                    Directory.CreateDirectory(options.TargetPath);

                    _logger.LogDebug(
                        "Target directory created {TargetPath}",
                        options.TargetPath);
                }

                using var image = new MagickImage(sourcePath);

                _logger.LogDebug(
                    "File loaded {SourcePath}",
                    sourcePath);

                token.ThrowIfCancellationRequested();

                string targetExtension =
                    ImageFormatHelper.GetExtension(options.TargetFormat);

                string fileNameWithoutExtension =
                    !string.IsNullOrWhiteSpace(options.OutputFileName)
                        ? options.OutputFileName
                        : Path.GetFileNameWithoutExtension(sourcePath);

                string targetPath = GetUniqueFilePath(
                    options.TargetPath,
                    fileNameWithoutExtension,
                    targetExtension);

                _logger.LogDebug(
                    "Output file {TargetPath}",
                    targetPath);

                image.Format = ImageFormatHelper.ToMagickFormat(options.TargetFormat);

                if(options.TargetFormat == ImageFormat.Webp || options.TargetFormat == ImageFormat.Jpg)
                    image.Quality = (uint)options.Quality;

                if (options.Width.HasValue || options.Height.HasValue)
                {
                    uint width = options.Width.HasValue
                        ? (uint)options.Width.Value
                        : image.Width;

                    uint height = options.Height.HasValue
                        ? (uint)options.Height.Value
                        : image.Height;

                    image.Resize(width, height);
                }

                //Jpg does not support transparency. The background is filled with custom color 
                if (options.TargetFormat == ImageFormat.Jpg)
                {
                    var backgroundColor = new MagickColor(
                        options.BackgroundColor.R,
                        options.BackgroundColor.G,
                        options.BackgroundColor.B);
                        
                    image.BackgroundColor = backgroundColor;
                    image.Alpha(AlphaOption.Remove);
                }

                if (!options.SaveMetadata)
                {
                    image.Strip();
                }

                token.ThrowIfCancellationRequested();

                image.Write(targetPath);

                _logger.LogDebug(
                    "File written {TargetPath}",
                    targetPath);

                return new ConversionResult(
                    true,
                    sourcePath,
                    targetPath,
                    null);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Error converting {SourcePath}",
                    sourcePath);

                return new ConversionResult(
                    false,
                    sourcePath,
                    null,
                    e.Message);
            }
        });
    }

    private static string GetUniqueFilePath(
        string directory,
        string fileName,
        string extension)
    {
        string path = Path.Combine(
            directory,
            $"{fileName}{extension}");

        for (int i = 1; File.Exists(path); i++)
        {
            path = Path.Combine(
                directory,
                $"{fileName}({i}){extension}");
        }

        return path;
    }
}