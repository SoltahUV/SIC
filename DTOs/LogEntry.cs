using System;
using SIC.Enums;

namespace SIC.DTOs;

public record LogEntry(
    LogLevel LogLevel,
    string Message,
    DateTime Timestamp
);