using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using SIC.Enums;

namespace SIC.Helpers;

public static class ImageFormatHelper
{
    private static readonly Dictionary<string, ImageFormat> ExtensionMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "png", ImageFormat.Png },
            { "webp", ImageFormat.Webp },
            { "avif", ImageFormat.Avif },
            { "heic", ImageFormat.Heic },
            { "heif", ImageFormat.Heic },
            { "jpg", ImageFormat.Jpg },
            { "jpeg", ImageFormat.Jpg },
            { "gif", ImageFormat.Gif },
            { "bmp", ImageFormat.Bmp },
            { "svg", ImageFormat.Svg },
            { "tiff", ImageFormat.Tiff },
            { "tif", ImageFormat.Tiff },
            { "ico", ImageFormat.Ico }
        };

    /// <summary>
    /// Attempts to determine the image format from the file extension.
    /// </summary>
    /// <param name="filePath">The path to the image file.</param>
    /// <param name="format">
    /// When this method returns, contains the detected image format.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the image format was successfully detected;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetFormat(
        string filePath,
        out ImageFormat format)
    {
        string ext = Path.GetExtension(filePath).TrimStart('.');

        return ExtensionMap.TryGetValue(ext, out format);
    }

    /// <summary>
    /// Converts an <see cref="ImageFormat"/> to the corresponding
    /// <see cref="MagickFormat"/>.
    /// </summary>
    /// <param name="format">The image format to convert.</param>
    /// <returns>The corresponding ImageMagick format.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the specified image format is not supported.
    /// </exception>
    public static MagickFormat ToMagickFormat(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Png => MagickFormat.Png,
            ImageFormat.Webp => MagickFormat.WebP,
            ImageFormat.Avif => MagickFormat.Avif,
            ImageFormat.Heic => MagickFormat.Heic,
            ImageFormat.Jpg => MagickFormat.Jpeg,
            ImageFormat.Gif => MagickFormat.Gif,
            ImageFormat.Bmp => MagickFormat.Bmp,
            ImageFormat.Tiff => MagickFormat.Tiff,
            ImageFormat.Ico => MagickFormat.Ico,

            _ => throw new NotSupportedException(
                $"Unsupported image format: {format}")
        };
    }

    /// <summary>
    /// Gets the default file extension for the specified image format.
    /// </summary>
    /// <param name="format">The image format.</param>
    /// <returns>The corresponding file extension, including the leading period.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the specified image format is not supported.
    /// </exception>
    public static string GetExtension(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Png => ".png",
            ImageFormat.Webp => ".webp",
            ImageFormat.Avif => ".avif",
            ImageFormat.Heic => ".heic",
            ImageFormat.Jpg => ".jpg",
            ImageFormat.Gif => ".gif",
            ImageFormat.Bmp => ".bmp",
            ImageFormat.Tiff => ".tiff",
            ImageFormat.Ico => ".ico",

            _ => throw new NotSupportedException(
                $"Unsupported image format: {format}")
        };
    }

    /// <summary>
    /// Gets the display name for the specified image format.
    /// </summary>
    /// <param name="format">The image format.</param>
    /// <returns>The formatted name of the image format.</returns>
    public static string GetDisplayName(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Png => "PNG",
            ImageFormat.Webp => "WebP",
            ImageFormat.Avif => "AVIF",
            ImageFormat.Heic => "HEIC",
            ImageFormat.Jpg => "JPEG",
            ImageFormat.Gif => "GIF",
            ImageFormat.Bmp => "BMP",
            ImageFormat.Tiff => "TIFF",
            ImageFormat.Ico => "ICO",

            _ => format.ToString()
        };
    }
}