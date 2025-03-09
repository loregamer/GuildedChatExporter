using System;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Infrastructure;
using GuildedChatExporter.Cli.Commands.Base;
using GuildedChatExporter.Cli.Utils.Extensions;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Commands;

[Command("getdms", Description = "Gets the list of direct message channels.")]
public class GetDirectChannelsCommand : GuildedCommandBase
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await base.ExecuteAsync(console);

        var cancellationToken = console.RegisterCancellationHandler();

        await console.Output.WriteLineAsync("Fetching direct message channels...");

        var fetchedChannelsCount = 0;
        await console
            .CreateProgressTicker()
            .StartAsync(async ctx =>
            {
                await foreach (var channel in Guilded.GetDirectChannelsAsync(cancellationToken))
                {
                    using (
                        GuildedChatExporter.Cli.Utils.Extensions.ConsoleExtensions.WithForegroundColor(
                            console,
                            ConsoleColor.White
                        )
                    )
                        await console.Output.WriteAsync($"{channel.Id}");

                    await console.Output.WriteLineAsync($" | {channel.DisplayName}");

                    ctx.Status(Markup.Escape($"Fetched '{channel.DisplayName}'."));

                    fetchedChannelsCount++;
                }
            });

        await console.Output.WriteLineAsync($"Fetched {fetchedChannelsCount} channel(s).");
    }
}
