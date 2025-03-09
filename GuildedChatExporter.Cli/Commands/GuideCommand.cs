using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace GuildedChatExporter.Cli.Commands;

[Command("guide", Description = "Shows information on how to use this program.")]
public class GuideCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("GuildedChatExporter Guide");
        console.Output.WriteLine("------------------------");
        console.Output.WriteLine();

        console.Output.WriteLine(
            "This program allows you to export message history from Guilded channels to a file."
        );
        console.Output.WriteLine();

        console.Output.WriteLine("Authentication:");
        console.Output.WriteLine(
            "  To use this program, you need to provide your Guilded authentication cookies."
        );
        console.Output.WriteLine("  You can get these by:");
        console.Output.WriteLine("  1. Opening Guilded in your browser");
        console.Output.WriteLine("  2. Logging in to your account");
        console.Output.WriteLine("  3. Opening browser developer tools (F12)");
        console.Output.WriteLine("  4. Going to the 'Network' tab");
        console.Output.WriteLine("  5. Refreshing the page");
        console.Output.WriteLine("  6. Clicking on any request to guilded.gg");
        console.Output.WriteLine("  7. Looking for the 'Cookie' header in the request headers");
        console.Output.WriteLine("  8. Copying the entire cookie string");
        console.Output.WriteLine();
        console.Output.WriteLine(
            "  You can then provide this cookie string using the --cookies option or the GUILDED_COOKIES environment variable."
        );
        console.Output.WriteLine();

        console.Output.WriteLine("Basic usage:");
        console.Output.WriteLine("  1. List all direct message channels:");
        console.Output.WriteLine(
            "     GuildedChatExporter.Cli getdms --cookies \"your_cookie_string\""
        );
        console.Output.WriteLine();
        console.Output.WriteLine("  2. Export a specific channel:");
        console.Output.WriteLine(
            "     GuildedChatExporter.Cli exportchannel --channel \"channel_id\" --cookies \"your_cookie_string\""
        );
        console.Output.WriteLine();
        console.Output.WriteLine("  3. Export all direct message channels:");
        console.Output.WriteLine(
            "     GuildedChatExporter.Cli exportdms --cookies \"your_cookie_string\""
        );
        console.Output.WriteLine();

        console.Output.WriteLine("Export formats:");
        console.Output.WriteLine("  You can specify the export format using the --format option:");
        console.Output.WriteLine("  - HtmlDark (default): HTML with dark theme");
        console.Output.WriteLine("  - HtmlLight: HTML with light theme");
        console.Output.WriteLine("  - PlainText: Plain text");
        console.Output.WriteLine("  - Json: JSON");
        console.Output.WriteLine("  - Csv: CSV");
        console.Output.WriteLine();

        console.Output.WriteLine("Output path:");
        console.Output.WriteLine("  You can specify the output path using the --output option:");
        console.Output.WriteLine(
            "  - If a directory is specified (path ends with a slash), files will be generated automatically based on channel names."
        );
        console.Output.WriteLine(
            "  - If a file is specified, the export will be saved to that file."
        );
        console.Output.WriteLine();

        console.Output.WriteLine("Media download:");
        console.Output.WriteLine(
            "  You can download media (avatars, attachments, etc.) using the --media option."
        );
        console.Output.WriteLine(
            "  Media files will be saved to a directory next to the export file, or to the directory specified by --media-dir."
        );
        console.Output.WriteLine();

        console.Output.WriteLine("For more information, use the --help option on any command.");

        return default;
    }
}
