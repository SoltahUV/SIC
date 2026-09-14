using System;
using System.Collections.Generic;
using SIC.DTOs;
using SIC.Enums;
using SIC.Services.Interfaces;

namespace SIC.Services;

public class LoggingService : ILoggingService
{
    private readonly List<LogEntry> _entries = new();

    public event Action<LogEntry>? LogAdded;

    public void AddLog(LogLevel logLevel, string message)
    {
        var entry = new LogEntry(logLevel, $"{message}", DateTime.Now);
        _entries.Add(entry);
        LogAdded?.Invoke(entry);
    }

    public void ClearLogs()
    {
        _entries.Clear();
    }

    public IReadOnlyList<LogEntry> GetAll() => _entries.AsReadOnly();
}