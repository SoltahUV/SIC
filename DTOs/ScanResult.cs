using System.Collections.Generic;

namespace SIC.DTOs;

public record ScanResult(IReadOnlyList<string> ValidFiles, int SkippedCount);