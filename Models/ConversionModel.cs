using SIC.Enums;
using SIC.DTOs;
using SIC.Services.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SIC.Models;

public class ConversionModel
{
    private readonly IConversionService _conversionService;
    private readonly ILoggingService _loggingService;
    private readonly IFileService _fileService;

    public ConversionModel(IConversionService conversionService,
        ILoggingService loggingService, IFileService fileService)
    {
        _conversionService = conversionService;
        _loggingService = loggingService;
        _fileService = fileService;

    }
    /// <summary>
    /// Converts a single image using the specified conversion options.
    /// </summary>
    /// <param name="sourcePath">The path to the source image.</param>
    /// <param name="options">The conversion options to apply.</param>
    /// <param name="cToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="BatchResult"/> containing the result of the conversion.
    /// </returns>
    public async Task<BatchResult> ConvertSingleAsync(string sourcePath, ConversionOptions options, CancellationToken cToken)
    {
        _loggingService.AddLog(LogLevel.Debug, $"Start conversion : {sourcePath}");
        ConversionResult result = await _conversionService.ConvertAsync(sourcePath, options, cToken);

        if (result.Success)
        {
            _loggingService.AddLog(LogLevel.Info, $"Complete conversion : {result.SourcePath} => {result.OutputPath}");
            return new BatchResult(1,0,0);
        }
        else
        {
            _loggingService.AddLog(LogLevel.Error, $"Error conversion : {result.SourcePath} [{result.ErrorMessage}]");
            return new BatchResult(0,0,1);
        }

    }

    /// <summary>
    /// Converts all supported images in the specified directory using the provided conversion options.
    /// </summary>
    /// <param name="sourcePath">The path to the directory containing the source images.</param>
    /// <param name="options">The conversion options to apply.</param>
    /// <param name="cToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="BatchResult"/> containing the results of the batch conversion.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the conversion operation is cancelled.
    /// </exception>
    public async Task<BatchResult> ConvertBatchAsync(string sourcePath, ConversionOptions options, CancellationToken cToken)
    {
        int totalSuccess = 0, totalFailed = 0;
        var createdFiles = new List<string>();

        _loggingService.AddLog(LogLevel.Debug, $"Scanning files in {sourcePath}");
        ScanResult scanResult = _fileService.Scan(sourcePath);
        IReadOnlyList<string> filesToConvert = scanResult.ValidFiles;

        _loggingService.AddLog(LogLevel.Info, $"Start batch conversion : {sourcePath}/files:{filesToConvert.Count}/skipped:{scanResult.SkippedCount}");

        try
        {

            foreach (var file in filesToConvert)
            {
                _loggingService.AddLog(LogLevel.Debug, $"File to convert: {file}");
                
                cToken.ThrowIfCancellationRequested();

                var result = await _conversionService.ConvertAsync(file, options, cToken);
                
                if (result.Success)
                {
                    totalSuccess++;
                    if (result.OutputPath != null) createdFiles.Add(result.OutputPath);
                    _loggingService.AddLog(LogLevel.Info, $"Complete conversion : {result.SourcePath} => {result.OutputPath}");
                }
                else
                {
                    totalFailed++;
                    _loggingService.AddLog(LogLevel.Error, $"Error conversion : {result.SourcePath} [{result.ErrorMessage}]");
                }
            }
        }
        // When canceled, all files that were changed during the operation are deleted
        catch (OperationCanceledException)
        {
            _loggingService.AddLog(LogLevel.Warning, "Operation cancelled by user. Rolling back...");
            foreach (var path in createdFiles)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);

                        _loggingService.AddLog(
                            LogLevel.Debug,
                            $"Deleted rollback file: {path}");
                    }
                }
                catch (Exception e)
                {
                    _loggingService.AddLog(
                        LogLevel.Error,
                        $"Failed to delete rollback file: {path}. {e.Message}");
                }
            }
            throw;
        }
        return new BatchResult(
            totalSuccess,
            scanResult.SkippedCount,
            totalFailed);
    }
}