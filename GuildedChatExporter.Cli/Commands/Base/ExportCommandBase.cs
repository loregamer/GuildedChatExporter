using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using Gress;
using GuildedChatExporter.Cli.Utils.Extensions;
using GuildedChatExporter.Core.Exceptions;
using GuildedChatExporter.Core.Exporting;
using GuildedChatExporter.Core.Guilded;
using GuildedChatExporter.Core.Guilded.Data;
using GuildedChatExporter.Core.Utils.Extensions;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Commands.Base;

public abstract class ExportCommandBase : GuildedCommandBase
{
    private readonly string _outputPath = Directory.GetCurrentDirectory();

    [CommandOption(
        "output",
        'o',
        Description = "Output file or directory path. "
            + "If a directory is specified, file names will be generated automatically based on the channel names and export parameters. "
            + "Directory paths must end with a slash to avoid ambiguity. "
            + "Supports template tokens, see the documentation for more info."
    )]
    public string OutputPath
    {
        get => _outputPath;
        // Handle ~/ in paths on Unix systems
        init => _outputPath = Path.GetFullPath(value);
    }

    [CommandOption("format", 'f', Description = "Export format.")]
    public ExportFormat ExportFormat { get; init; } = ExportFormat.HtmlDark;

    [CommandOption(
        "markdown",
        Description = "Process markdown, mentions, and other special tokens."
    )]
    public bool ShouldFormatMarkdown { get; init; } = true;

    [CommandOption(
        "media",
        Description = "Download assets referenced by the export (user avatars, attached files, embedded images, etc.)."
    )]
    public bool ShouldDownloadAssets { get; init; }

    [CommandOption(
        "reuse-media",
        Description = "Reuse previously downloaded assets to avoid redundant requests."
    )]
    public bool ShouldReuseAssets { get; init; } = false;

    private readonly string? _assetsDirPath;

    [CommandOption(
        "media-dir",
        Description = "Download assets to this directory. "
            + "If not specified, the asset directory path will be derived from the output path."
    )]
    public string? AssetsDirPath
    {
        get => _assetsDirPath;
        // Handle ~/ in paths on Unix systems
        init => _assetsDirPath = value is not null ? Path.GetFullPath(value) : null;
    }

    [CommandOption(
        "locale",
        Description = "Locale to use when formatting dates and numbers. "
            + "If not specified, the default system locale will be used."
    )]
    public string? Locale { get; init; }

    [CommandOption("utc", Description = "Normalize all timestamps to UTC+0.")]
    public bool IsUtcNormalizationEnabled { get; init; } = false;

    [CommandOption(
        "parallel",
        Description = "Limits how many channels can be exported in parallel."
    )]
    public int ParallelLimit { get; init; } = 1;

    private ChannelExporter? _channelExporter;
    protected ChannelExporter Exporter => _channelExporter ??= new ChannelExporter(Guilded);

    protected async ValueTask ExportAsync(IConsole console, IReadOnlyList<Channel> channels)
    {
        // Asset reuse can only be enabled if the download assets option is set
        if (ShouldReuseAssets && !ShouldDownloadAssets)
        {
            throw new CommandException("Option --reuse-media cannot be used without --media.");
        }

        // Assets directory can only be specified if the download assets option is set
        if (!string.IsNullOrWhiteSpace(AssetsDirPath) && !ShouldDownloadAssets)
        {
            throw new CommandException("Option --media-dir cannot be used without --media.");
        }

        // Make sure the user does not try to export multiple channels into one file.
        // Output path must either be a directory or contain template tokens for this to work.
        var isValidOutputPath =
            // Anything is valid when exporting a single channel
            channels.Count <= 1
            // When using template tokens, assume the user knows what they're doing
            || OutputPath.Contains('%')
            // Otherwise, require an existing directory or an unambiguous directory path
            || Directory.Exists(OutputPath)
            || Path.EndsInDirectorySeparator(OutputPath);

        if (!isValidOutputPath)
        {
            throw new CommandException(
                "Attempted to export multiple channels, but the output path is neither a directory nor a template. "
                    + "If the provided output path is meant to be treated as a directory, make sure it ends with a slash. "
                    + $"Provided output path: '{OutputPath}'."
            );
        }

        // Export
        var cancellationToken = console.RegisterCancellationHandler();
        var errorsByChannel = new ConcurrentDictionary<Channel, string>();

        await console.Output.WriteLineAsync($"Exporting {channels.Count} channel(s)...");
        await console
            .CreateProgressTicker()
            .HideCompleted(
                // When exporting multiple channels in parallel, hide the completed tasks
                // because it gets hard to visually parse them as they complete out of order.
                ParallelLimit > 1
            )
            .StartAsync(async ctx =>
            {
                await Parallel.ForEachAsync(
                    channels,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Math.Max(1, ParallelLimit),
                        CancellationToken = cancellationToken,
                    },
                    async (channel, innerCancellationToken) =>
                    {
                        try
                        {
                            await ctx.StartTaskAsync(
                                Markup.Escape(channel.DisplayName),
                                async progress =>
                                {
                                    var request = new ExportRequest(
                                        channel,
                                        OutputPath,
                                        AssetsDirPath,
                                        ExportFormat,
                                        ShouldFormatMarkdown,
                                        ShouldDownloadAssets,
                                        ShouldReuseAssets,
                                        Locale,
                                        IsUtcNormalizationEnabled
                                    );

                                    await Exporter.ExportChannelAsync(
                                        request,
                                        progress.ToPercentageBased(),
                                        innerCancellationToken
                                    );
                                }
                            );
                        }
                        catch (GuildedChatExporterException ex) when (!ex.IsFatal)
                        {
                            errorsByChannel[channel] = ex.Message;
                        }
                    }
                );
            });

        // Print the result
        using (
            GuildedChatExporter.Cli.Utils.Extensions.ConsoleExtensions.WithForegroundColor(
                console,
                ConsoleColor.White
            )
        )
        {
            await console.Output.WriteLineAsync(
                $"Successfully exported {channels.Count - errorsByChannel.Count} channel(s)."
            );
        }

        // Print errors
        if (errorsByChannel.Any())
        {
            await console.Output.WriteLineAsync();

            using (
                GuildedChatExporter.Cli.Utils.Extensions.ConsoleExtensions.WithForegroundColor(
                    console,
                    ConsoleColor.Red
                )
            )
            {
                await console.Error.WriteLineAsync(
                    $"Failed to export {errorsByChannel.Count} the following channel(s):"
                );
            }

            foreach (var (channel, error) in errorsByChannel)
            {
                await console.Error.WriteAsync($"{channel.DisplayName}: ");
                using (
                    GuildedChatExporter.Cli.Utils.Extensions.ConsoleExtensions.WithForegroundColor(
                        console,
                        ConsoleColor.Red
                    )
                )
                    await console.Error.WriteLineAsync(error);
            }

            await console.Error.WriteLineAsync();
        }

        // Fail the command only if ALL channels failed to export.
        // If only some channels failed to export, it's okay.
        if (errorsByChannel.Count >= channels.Count)
            throw new CommandException("Export failed.");
    }

    protected async ValueTask ExportAsync(IConsole console, IReadOnlyList<GuildedId> channelIds)
    {
        var cancellationToken = console.RegisterCancellationHandler();

        await console.Output.WriteLineAsync("Resolving channel(s)...");

        var channels = new List<Channel>();

        foreach (var channelId in channelIds)
        {
            var channel = await Guilded.GetChannelAsync(channelId, cancellationToken);
            channels.Add(channel);
        }

        await ExportAsync(console, channels);
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await base.ExecuteAsync(console);
    }
}
