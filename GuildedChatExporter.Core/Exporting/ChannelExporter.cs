using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using GuildedChatExporter.Core.Guilded;

namespace GuildedChatExporter.Core.Exporting;

public class ChannelExporter
{
    private readonly GuildedClient _guilded;

    public ChannelExporter(GuildedClient guilded)
    {
        _guilded = guilded;
    }

    public async Task ExportChannelAsync(
        ExportRequest request,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default
    )
    {
        // Get the output path
        var outputPath = request.GetOutputPath();

        // Create the directory if it doesn't exist
        var outputDirPath = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outputDirPath))
            Directory.CreateDirectory(outputDirPath);

        // Create the assets directory if needed
        var assetsDirPath = request.GetAssetsDirPath();
        if (!string.IsNullOrWhiteSpace(assetsDirPath))
            Directory.CreateDirectory(assetsDirPath);

        // Export the channel
        await using var output = File.Create(outputPath);
        await using var writer = new StreamWriter(output, Encoding.UTF8);

        // For now, just write a simple text file with the channel info
        await writer.WriteLineAsync($"Channel: {request.Channel.DisplayName}");
        await writer.WriteLineAsync($"ID: {request.Channel.Id}");
        await writer.WriteLineAsync($"Type: {request.Channel.Type}");
        await writer.WriteLineAsync($"Created At: {request.Channel.CreatedAt}");
        await writer.WriteLineAsync($"Created By: {request.Channel.CreatedBy}");
        await writer.WriteLineAsync($"Updated At: {request.Channel.UpdatedAt}");
        await writer.WriteLineAsync();

        // Export messages
        await writer.WriteLineAsync("Messages:");
        await writer.WriteLineAsync();

        await foreach (
            var message in _guilded.GetMessagesAsync(
                request.Channel.Id,
                progress,
                cancellationToken
            )
        )
        {
            await writer.WriteLineAsync(
                $"[{message.CreatedAt}] {message.CreatedBy}: {message.GetPlainTextContent()}"
            );
        }
    }
}
