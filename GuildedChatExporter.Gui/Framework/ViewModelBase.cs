using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GuildedChatExporter.Gui.Framework;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    private bool _isDisposed;

    ~ViewModelBase() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
    }
}
