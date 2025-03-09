using System.Collections.Generic;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;
using GuildedChatExporter.Cli.Commands.Base;
using GuildedChatExporter.Cli.Utils.Extensions;
using GuildedChatExporter.Core.Guilded.Data;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Commands;

[Command("exportdms", Description = "Exports all direct message channels.")]
public class ExportDirectMessagesCommand : ExportCommandBase
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await base.ExecuteAsync(console);

        var cancellationToken = console.RegisterCancellationHandler();
        var channels = new List<Channel>();

        await console.Output.WriteLineAsync("Fetching direct message channels...");

        var fetchedChannelsCount = 0;
        await console
            .CreateProgressTicker()
            .StartAsync(async ctx =>
            {
                await foreach (var channel in Guilded.GetDirectChannelsAsync(cancellationToken))
                {
                    channels.Add(channel);

                    ctx.Status(Markup.Escape($"Fetched '{channel.DisplayName}'."));

                    fetchedChannelsCount++;
                }
            });

        await console.Output.WriteLineAsync($"Fetched {fetchedChannelsCount} channel(s).");

        await ExportAsync(console, channels);
    }
}
