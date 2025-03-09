using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;
using GuildedChatExporter.Cli.Commands.Base;
using GuildedChatExporter.Core.Guilded;
using GuildedChatExporter.Core.Guilded.Data;

namespace GuildedChatExporter.Cli.Commands;

[Command("exportdms", Description = "Exports all direct message channels.")]
public class ExportDirectMessagesCommand : ExportCommandBase
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await base.ExecuteAsync(console);

        var cancellationToken = console.RegisterCancellationHandler();

        await console.Output.WriteLineAsync("Fetching direct message channels...");

        var channels = new List<Channel>();
        await foreach (var channel in Guilded.GetDirectChannelsAsync(cancellationToken))
        {
            channels.Add(channel);
        }

        if (!channels.Any())
        {
            await console.Output.WriteLineAsync("No direct message channels found.");
            return;
        }

        await console.Output.WriteLineAsync($"Found {channels.Count} channel(s).");
        await ExportAsync(console, channels);
    }
}
