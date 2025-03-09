using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Commands;

[Command("guide", Description = "Prints usage guide.")]
public class GuideCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        var markup = new Markup(
            @"[bold yellow]GuildedChatExporter[/] is a tool that allows you to export message history from Guilded channels to a file.

[bold]Authentication[/]

To use this program, you need to provide your Guilded authentication cookies. You can get these by:

1. Opening Guilded in your browser
2. Logging in to your account
3. Opening browser developer tools (F12)
4. Going to the 'Network' tab
5. Refreshing the page
6. Clicking on any request to guilded.gg
7. Looking for the 'Cookie' header in the request headers
8. Copying the entire cookie string

You can then provide this cookie string using the [bold]--cookies[/] option or the [bold]GUILDED_COOKIES[/] environment variable.

[bold]Basic usage[/]

1. List all direct message channels:

   [grey]GuildedChatExporter.Cli getdms --cookies ""your_cookie_string""[/]

2. Export a specific channel:

   [grey]GuildedChatExporter.Cli exportchannel --channel ""channel_id"" --cookies ""your_cookie_string""[/]

3. Export all direct message channels:

   [grey]GuildedChatExporter.Cli exportdms --cookies ""your_cookie_string""[/]

For more information, use the [bold]--help[/] option with any command."
        );

        AnsiConsole.Write(markup);
        return default;
    }
}
