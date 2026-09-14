namespace SIC.DTOs;

public record ConversionResult(bool Success, string SourcePath, 
    string? OutputPath, string? ErrorMessage);