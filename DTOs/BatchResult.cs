namespace SIC.DTOs;
public record BatchResult(int SuccessCount, 
    int SkippedCount, int FailedCount);