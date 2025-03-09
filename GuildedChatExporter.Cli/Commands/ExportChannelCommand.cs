using System.Collections.Generic;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;
using GuildedChatExporter.Cli.Commands.Base;
using GuildedChatExporter.Core.Guilded;

namespace GuildedChatExporter.Cli.Commands;

[Command("exportchannel", Description = "Exports a channel.")]
public class ExportChannelCommand : ExportCommandBase
{
    [CommandOption("channel", 'c', Description = "Channel ID.")]
    public string? ChannelId { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await base.ExecuteAsync(console);

        if (string.IsNullOrWhiteSpace(ChannelId))
        {
            await console.Output.WriteLineAsync("No channel ID specified.");
            return;
        }

        var channelIds = new List<GuildedId> { new GuildedId(ChannelId) };
        await ExportAsync(console, channelIds);
    }
}
