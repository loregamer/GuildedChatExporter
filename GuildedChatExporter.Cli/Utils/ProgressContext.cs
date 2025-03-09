using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Utils;

public class ProgressContext
{
    private readonly IAnsiConsole _console;
    private readonly TextWriter _output;
    private readonly bool _isOutputRedirected;

    public ProgressContext(TextWriter output, bool isOutputRedirected)
    {
        _console = AnsiConsole.Create(
            new AnsiConsoleSettings { Out = new AnsiConsoleOutput(output) }
        );
        _output = output;
        _isOutputRedirected = isOutputRedirected;
    }

    public ProgressContext HideCompleted(bool value)
    {
        // This is a no-op for now
        return this;
    }

    public async Task StartAsync(Func<ProgressContext, Task> handler)
    {
        if (_isOutputRedirected)
        {
            await handler(this);
            return;
        }

        await _console
            .Progress()
            .AutoClear(true)
            .HideCompleted(false)
            .Columns(
                new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn(),
                }
            )
            .StartAsync(async ctx => await handler(this));
    }

    public async Task StartTaskAsync(string description, Func<IProgress<double>, Task> handler)
    {
        if (_isOutputRedirected)
        {
            await _output.WriteLineAsync($"Starting task: {description}");
            var progress = new Progress<double>(p =>
            {
                if (p >= 1.0)
                {
                    _output.WriteLineAsync($"Completed task: {description}");
                }
            });

            await handler(progress);
            return;
        }

        var task = new ProgressTask(description);
        var progressHandler = new Progress<double>(p => task.Value = p * 100);
        await handler(progressHandler);
    }

    public void Status(string status)
    {
        if (_isOutputRedirected)
        {
            _output.WriteLineAsync(status);
        }
    }

    private class ProgressTask
    {
        public string Description { get; }
        public double Value { get; set; }

        public ProgressTask(string description)
        {
            Description = description;
        }
    }

    private class AnsiConsoleOutput : IAnsiConsoleOutput
    {
        private readonly TextWriter _writer;

        public AnsiConsoleOutput(TextWriter writer)
        {
            _writer = writer;
        }

        public TextWriter Writer => _writer;

        public bool IsTerminal => false;

        public int Width => 80;

        public int Height => 24;

        public void SetEncoding(Encoding encoding)
        {
            // No-op
        }
    }
}
