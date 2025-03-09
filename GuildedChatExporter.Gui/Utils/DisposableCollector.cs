using System;
using System.Collections.Generic;

namespace GuildedChatExporter.Gui.Utils;

public class DisposableCollector : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private bool _isDisposed;

    public void Add(IDisposable disposable) => _disposables.Add(disposable);

    public void Dispose()
    {
        if (_isDisposed)
            return;

        foreach (var disposable in _disposables)
            disposable.Dispose();

        _disposables.Clear();
        _isDisposed = true;
    }
}
