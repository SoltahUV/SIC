using System;

namespace SIC.Exceptions;

public class InvalidImageFileException : Exception
{
    public string FilePath { get; }

    public InvalidImageFileException(string filePath, string message, Exception innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }
}