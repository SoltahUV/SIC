using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIC.DTOs;
using SIC.Enums;
using SIC.Exceptions;
using SIC.Helpers;
using SIC.Models;
using SIC.Services.Interfaces;

namespace SIC.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ConversionModel _conversionModel;
    private readonly ILoggingService _logger;
    private readonly IFileService _fileService;

    public LogsPanelViewModel Logs { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartConversionCommand))]
    private string? _sourcePath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartConversionCommand))]
    [NotifyPropertyChangedFor(nameof(ResultPath))]
    private string? _targetPath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResultPath))]
    private string? _outputFileName;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsQualitySupported))]
    [NotifyPropertyChangedFor(nameof(IsBackgroundSupported))]
    [NotifyPropertyChangedFor(nameof(ResultPath))]
    private ImageFormat _selectedTargetFormat = ImageFormat.Png;

    public string? ResultPath =>
        IsFolder
            ? TargetPath
            : (!string.IsNullOrWhiteSpace(TargetPath) && !string.IsNullOrWhiteSpace(OutputFileName)
                ? Path.Combine(TargetPath, $"{OutputFileName}{ImageFormatHelper.GetExtension(SelectedTargetFormat)}")
                : null);


    public bool IsQualitySupported =>
        SelectedTargetFormat == ImageFormat.Jpg ||
        SelectedTargetFormat == ImageFormat.Webp;
    public bool IsBackgroundSupported =>
        SelectedTargetFormat == ImageFormat.Jpg;

    public bool IsResolutionPreviewEnabled =>
        !IsFolder;

    [ObservableProperty]
    private int _quality = 85;

    [ObservableProperty]
    private Color _backgroundColor = Colors.Black;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageWidth))]
    [NotifyPropertyChangedFor(nameof(ImageHeight))]
    private int _originalWidth;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageWidth))]
    [NotifyPropertyChangedFor(nameof(ImageHeight))]
    private int _originalHeight;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageWidth))]
    [NotifyPropertyChangedFor(nameof(ImageHeight))]
    private int _resizePercent = 100;
    public int MinResizePercent => 10;

    public int MaxResizePercent => 200;

    public int ImageWidth =>
        OriginalWidth > 0
            ? (int)(OriginalWidth * ResizePercent / 100.0)
            : 0;

    public int ImageHeight =>
        OriginalHeight > 0
            ? (int)(OriginalHeight * ResizePercent / 100.0)
            : 0;

    [ObservableProperty]
    private bool _saveMetadata = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartConversionCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsResolutionPreviewEnabled))]
    [NotifyPropertyChangedFor(nameof(ResultPath))]
    private bool _isFolder = true;

    private CancellationTokenSource? _cts;

    public IEnumerable<ImageFormat> AvailableFormats =>
     Enum.GetValues<ImageFormat>()
         .Where(format => format != ImageFormat.Svg
         && format != ImageFormat.Unknown);

    public MainViewModel(
        ConversionModel conversionModel,
        ILoggingService logger,
        IFileService fileService,
        LogsPanelViewModel logs)
    {
        _conversionModel = conversionModel;
        _logger = logger;
        _fileService = fileService;
        Logs = logs;
    }

    [RelayCommand]
    private async Task SelectSourceFileAsync(Window window)
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select file",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Images")
                {
                    Patterns = new[]
                        {
                            "*.jpg",
                            "*.jpeg",
                            "*.webp",
                            "*.bmp",
                            "*.tiff",
                            "*.gif",
                            "*.png"
                        }
                }
            }
        };

        var files = await window.StorageProvider.OpenFilePickerAsync(options);
        if (files.Count > 0)
        {
            SetSourceFromPath(files[0].Path.LocalPath);
        }
    }

    [RelayCommand]
    private async Task SelectSourceFolderAsync(Window window)
    {
        var folder = await PickFolderAsync(
            window,
            "Select folder");

        if (!string.IsNullOrWhiteSpace(folder))
        {
            SetSourceFromPath(folder);
        }
    }
    // вызывается из view для drag-and-drop
    public void SetSourceFromPath(string path)
    {
        var isDirectory = Directory.Exists(path);
        _logger.AddLog(LogLevel.Debug, $"Set file path : {path}");
        if (!isDirectory)
        {
            try
            {
                var fileInfo = _fileService.GetFileInfo(path);
                OriginalHeight = fileInfo.Height;
                OriginalWidth = fileInfo.Width;
            }
            catch(InvalidImageFileException ex)
            {
                _logger.AddLog(LogLevel.Error, ex.Message);
                return;
            }
        }
        SourcePath = path;
        TargetPath = Path.GetDirectoryName(path);
        OutputFileName = isDirectory ? null : Path.GetFileNameWithoutExtension(path);
        IsFolder = isDirectory;
    }
    [RelayCommand]
    private async Task SelectTargetFolderAsync(Window window)
    {
        TargetPath = await PickFolderAsync(
            window, 
            "Select folder to save");
    }

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task StartConversionAsync()
    {
        BatchResult result;
        IsBusy = true;
        _cts = new CancellationTokenSource();

        try
        {
            var options = new ConversionOptions(
                TargetFormat: SelectedTargetFormat,
                TargetPath: TargetPath!,
                Quality: Quality,
                BackgroundColor: BackgroundColor,
                SaveMetadata: SaveMetadata,
                Width: ImageWidth > 0 ? ImageWidth : null,
                Height: ImageHeight > 0 ? ImageHeight : null,
                OutputFileName: IsFolder ? null : OutputFileName

            );
            if (IsFolder)
            {
                result = await _conversionModel.ConvertBatchAsync(SourcePath!, options, _cts.Token);
            }
            else
            {
                result = await _conversionModel.ConvertSingleAsync(SourcePath!, options, _cts.Token);
            }
            _logger.AddLog(
                LogLevel.Info,
                $"Conversion Ended. " +
                $"Successful: {result.SuccessCount}, " +
                $"Skipped: {result.SkippedCount}, " +
                $"Errors: {result.FailedCount}");
        }
        catch (OperationCanceledException)
        {
            _logger.AddLog(
                LogLevel.Warning,
                "Conversion cancelled");
        }
        finally
        {
            IsBusy = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    private bool CanConvert() => !IsBusy 
                                 && !string.IsNullOrWhiteSpace(SourcePath) 
                                 && !string.IsNullOrWhiteSpace(TargetPath);

    private static async Task<string?> PickFolderAsync(Window window, string title)
    {
        var folders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });
        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }

}