using Avalonia.Media;
using SIC.Enums;

namespace SIC.DTOs;
public record ConversionOptions(
    ImageFormat TargetFormat,
    string TargetPath,
    int Quality,
    Color BackgroundColor,
    bool SaveMetadata,
    int? Width,
    int? Height,
    string? OutputFileName = null
);