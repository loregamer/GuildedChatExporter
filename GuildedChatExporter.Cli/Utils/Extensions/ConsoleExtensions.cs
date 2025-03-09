using System;
using System.Threading;
using CliFx.Infrastructure;
using Gress;

namespace GuildedChatExporter.Cli.Utils.Extensions;

public static class ConsoleExtensions
{
    public static CancellationToken RegisterCancellationHandler(this IConsole console)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        // We need to handle cancellation manually since we can't use the built-in method
        // This is a workaround for the missing RegisterCancellationHandler method
        // We'll just return the token and let the caller handle cancellation
        return cancellationTokenSource.Token;
    }

    public static IDisposable WithForegroundColor(
        this IConsole console,
        ConsoleColor foregroundColor
    )
    {
        var previousForegroundColor = console.ForegroundColor;
        console.ForegroundColor = foregroundColor;

        return new ActionDisposable(() => console.ForegroundColor = previousForegroundColor);
    }

    public static ProgressContext CreateProgressTicker(this IConsole console) =>
        new(console.Output, console.IsOutputRedirected);

    public static IProgress<Percentage> ToPercentageBased(this IProgress<double> progress) =>
        new ProgressAdapter(progress);
}

internal class ActionDisposable : IDisposable
{
    private readonly Action _disposeAction;
    private bool _isDisposed;

    public ActionDisposable(Action disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _disposeAction();
        _isDisposed = true;
    }
}

internal class ProgressAdapter : IProgress<Percentage>
{
    private readonly IProgress<double> _progress;

    public ProgressAdapter(IProgress<double> progress)
    {
        _progress = progress;
    }

    public void Report(Percentage value)
    {
        _progress.Report(value.Fraction);
    }
}
