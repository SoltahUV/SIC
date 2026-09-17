using System.Collections.Generic;
using System.IO;
using SIC.DTOs;
using SIC.Services.Interfaces;
using SIC.Helpers;
using ImageMagick;

using SIC.Enums;
using SIC.Exceptions;

namespace SIC.Services;

public class FileService : IFileService
{
    public ScanResult Scan(string folderPath)
    {
        int skippedCount = 0;

        string[] files = Directory.GetFiles(folderPath);

        List<string> validFiles = new();

        foreach (var item in files)
        {
            bool isValid = ImageFormatHelper.TryGetFormat(item, out _);
            if(isValid)
                validFiles.Add(item);
            else
                skippedCount++;
        }

        return new ScanResult(validFiles, skippedCount);
    }
    public ImageInfo GetFileInfo(string path)
    {
        try
        {

            using var image = new MagickImage(path);
            var fileInfo = new FileInfo(path);

            double sizeInMb = fileInfo.Length / (1024.0 * 1024.0);

            ImageFormat format = ImageFormatHelper.TryGetFormat(
                path,
                out var detectedFormat)
                ? detectedFormat
                : ImageFormat.Unknown;

            return new ImageInfo(
                (int)image.Width,
                (int)image.Height,
                format,
                sizeInMb);
        }
        catch (MagickException ex)
        {
            throw new InvalidImageFileException(path, $"Invalid image file : {path}", ex);
        }
    }
}