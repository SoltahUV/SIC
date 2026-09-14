using System;
using System.Collections.Generic;
using SIC.DTOs;
using SIC.Enums;

namespace SIC.Services.Interfaces;

public interface ILoggingService
{
    /// <summary>
    /// Occurs when a new log entry is added
    /// </summary>
    event Action<LogEntry>? LogAdded;

    /// <summary>
    /// Add a new log entry
    /// </summary>
    /// <param name="logLevel">The severiry level of the log enrty</param>
    /// <param name="message">The message to add to log</param>
    void AddLog(LogLevel logLevel, string message);

    /// <summary>
    /// Get read only log entries list
    /// </summary>
    /// <returns>Read only log enrties list</returns>
    IReadOnlyList<LogEntry> GetAll();
    void ClearLogs();
}