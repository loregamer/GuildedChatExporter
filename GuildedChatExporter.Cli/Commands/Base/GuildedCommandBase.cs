using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using GuildedChatExporter.Core.Guilded;

namespace GuildedChatExporter.Cli.Commands.Base;

public abstract class GuildedCommandBase : ICommand
{
    private readonly string? _cookiesEnvVar = Environment.GetEnvironmentVariable("GUILDED_COOKIES");

    [CommandOption(
        "cookies",
        Description = "Authentication cookies used to access Guilded. "
            + "You can also set the GUILDED_COOKIES environment variable."
    )]
    public string? Cookies { get; init; }

    protected GuildedClient Guilded { get; private set; } = null!;

    public virtual async ValueTask ExecuteAsync(IConsole console)
    {
        var cookies = Cookies ?? _cookiesEnvVar;

        if (string.IsNullOrWhiteSpace(cookies))
        {
            throw new CommandException(
                "Authentication cookies not provided. "
                    + "Set them using the --cookies option or the GUILDED_COOKIES environment variable."
            );
        }

        Guilded = new GuildedClient(cookies);

        // Verify that the cookies are valid
        try
        {
            await Guilded.GetUserIdAsync();
        }
        catch (Exception ex)
        {
            throw new CommandException(
                $"Failed to authenticate with the provided cookies: {ex.Message}"
            );
        }
    }
}
