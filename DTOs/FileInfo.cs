using SIC.Enums;

namespace SIC.DTOs;

public record ImageInfo(int Width, int Height, ImageFormat format, double size);