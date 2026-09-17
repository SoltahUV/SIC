using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIC.DTOs;
using SIC.Enums;
using SIC.Services.Interfaces;

namespace SIC.ViewModels;

public partial class LogsPanelViewModel : ViewModelBase, IDisposable
{
    private readonly ILoggingService _loggingService;

    private bool _disposed;

    /// <summary>
    /// Get availavle log levels
    /// </summary>
    public ObservableCollection<LogLevel> AvailableLogLevels { get; } =
    [
        LogLevel.All,
        LogLevel.Debug,
        LogLevel.Info,
        LogLevel.Warning,
        LogLevel.Error
    ];


    [ObservableProperty]
    private LogLevel _selectedLogLevel = LogLevel.Info;

    public ObservableCollection<LogEntry> FilteredLogs { get; } = new();


    public LogsPanelViewModel(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        RebuildFilteredLogs();

        _loggingService.LogAdded += OnLogAdded;
    }


    partial void OnSelectedLogLevelChanged(LogLevel value)
    {
        RebuildFilteredLogs();
    }


    private void RebuildFilteredLogs()
    {
        if (_disposed)
            return;


        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed)
                return;


            FilteredLogs.Clear();


            foreach (var entry in _loggingService
                         .GetAll()
                         .Where(PassesCurrentFilter))
            {
                FilteredLogs.Add(entry);
            }

        });
    }


    private void OnLogAdded(LogEntry entry)
    {
        if (_disposed)
            return;


        Dispatcher.UIThread.Post(() =>
        {
            if (_disposed)
                return;


            if (PassesCurrentFilter(entry))
            {
                FilteredLogs.Add(entry);
            }

            const int maxLogs = 5000;

            while (FilteredLogs.Count > maxLogs)
            {
                FilteredLogs.RemoveAt(0);
            }

        });
    }


    private bool PassesCurrentFilter(LogEntry entry)
    {
        if (SelectedLogLevel == LogLevel.All)
            return true;


        return entry.LogLevel >= SelectedLogLevel;
    }


    [RelayCommand]
    private void Clear()
    {
        FilteredLogs.Clear();
        _loggingService.ClearLogs();

    }


    public void Dispose()
    {
        if (_disposed)
            return;


        _disposed = true;

        _loggingService.LogAdded -= OnLogAdded;
    }
}