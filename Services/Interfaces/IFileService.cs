using SIC.DTOs;

namespace SIC.Services.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Scans the folder and receives a list of files without subfolders
    /// </summary>
    ScanResult Scan(string folderPath);
    
    /// <summary>
    /// Receives information about the file
    /// </summary>
    ImageInfo GetFileInfo(string path);
}