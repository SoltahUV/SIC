using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SIC.Enums;
using SIC.Helpers;

namespace SIC.Converters;

public class ImageFormatConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is ImageFormat format)
        {
            return ImageFormatHelper.GetDisplayName(format);
        }

        return string.Empty;
    }


    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}