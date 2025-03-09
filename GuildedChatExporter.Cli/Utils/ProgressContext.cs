using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace GuildedChatExporter.Cli.Utils;

public class ProgressContext
{
    private readonly TextWriter _output;
    private readonly bool _isOutputRedirected;
    private readonly Dictionary<string, ProgressTask> _tasks = new(StringComparer.Ordinal);
    private readonly object _lock = new();
    private bool _hideCompleted;

    public ProgressContext(TextWriter output, bool isOutputRedirected)
    {
        _output = output;
        _isOutputRedirected = isOutputRedirected;
    }

    public ProgressContext HideCompleted(bool hideCompleted = true)
    {
        _hideCompleted = hideCompleted;
        return this;
    }

    public async Task StartAsync(Func<ProgressContext, Task> handler)
    {
        if (_isOutputRedirected)
        {
            await handler(this);
            return;
        }

        await AnsiConsole
            .Progress()
            .AutoClear(false)
            .HideCompleted(_hideCompleted)
            .StartAsync(async ctx =>
            {
                var progressTasks = new Dictionary<string, Spectre.Console.ProgressTask>(
                    StringComparer.Ordinal
                );

                void RenderTask(string description, double? progress)
                {
                    if (!progressTasks.TryGetValue(description, out var progressTask))
                    {
                        progressTask = ctx.AddTask(description);
                        progressTasks[description] = progressTask;
                    }

                    if (progress is not null)
                        progressTask.Value = progress.Value * 100;
                }

                void RenderTasks()
                {
                    lock (_lock)
                    {
                        foreach (var (description, task) in _tasks)
                            RenderTask(description, task.Progress);
                    }
                }

                // Render initial state
                RenderTasks();

                // Start the handler
                await handler(this);

                // Mark all tasks as completed
                foreach (var task in progressTasks.Values)
                    task.Value = 100;
            });
    }

    public async Task StartTaskAsync(string description, Func<IProgress<double>, Task> handler)
    {
        var task = new ProgressTask(description);

        lock (_lock)
            _tasks[description] = task;

        try
        {
            await handler(task);
            task.Progress = 1;
        }
        catch
        {
            task.Progress = null;
            throw;
        }
        finally
        {
            if (_isOutputRedirected)
                await _output.WriteLineAsync($"Completed: {description}");
        }
    }

    public void Status(string status)
    {
        if (_isOutputRedirected)
            _output.WriteLine(status);
    }

    private class ProgressTask : IProgress<double>
    {
        public string Description { get; }
        public double? Progress { get; set; }

        public ProgressTask(string description)
        {
            Description = description;
        }

        public void Report(double value) => Progress = value;
    }
}
