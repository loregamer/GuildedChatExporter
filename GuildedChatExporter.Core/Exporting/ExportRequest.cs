using System.IO;
using GuildedChatExporter.Core.Guilded.Data;

namespace GuildedChatExporter.Core.Exporting;

public record ExportRequest(
    Channel Channel,
    string OutputPath,
    string? AssetsDirPath,
    ExportFormat Format,
    bool ShouldFormatMarkdown,
    bool ShouldDownloadAssets,
    bool ShouldReuseAssets,
    string? Locale,
    bool IsUtcNormalizationEnabled
)
{
    public string GetOutputPath()
    {
        // If the output path is a directory, generate a file name based on the channel
        if (Directory.Exists(OutputPath) || Path.EndsInDirectorySeparator(OutputPath))
        {
            var dirPath = OutputPath;
            var fileName = $"{Channel.DisplayName} [{Channel.Id}]";

            return Path.Combine(dirPath, fileName + Format.GetFileExtension());
        }

        // Otherwise, use the output path as is
        return OutputPath;
    }

    public string? GetAssetsDirPath()
    {
        if (!ShouldDownloadAssets)
            return null;

        if (!string.IsNullOrWhiteSpace(AssetsDirPath))
            return AssetsDirPath;

        var outputPath = GetOutputPath();
        var outputDirPath = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
        var outputFileName = Path.GetFileNameWithoutExtension(outputPath);

        return Path.Combine(outputDirPath, outputFileName);
    }
}
