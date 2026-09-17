using System.Threading;
using System.Threading.Tasks;
using SIC.DTOs;

namespace SIC.Services.Interfaces;

public interface IConversionService
{
    /// <summary>
    /// Converts an image using the specified options.
    /// </summary>
    Task<ConversionResult> ConvertAsync(string sourcePath, 
        ConversionOptions options, CancellationToken token);
}