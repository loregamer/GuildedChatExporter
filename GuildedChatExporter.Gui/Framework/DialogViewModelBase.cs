using System;

namespace GuildedChatExporter.Gui.Framework;

public abstract class DialogViewModelBase : ViewModelBase
{
    public event EventHandler<DialogCloseRequestedEventArgs>? CloseRequested;

    protected void Close(bool? dialogResult = null) =>
        CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(dialogResult));
}

public class DialogCloseRequestedEventArgs(bool? dialogResult) : EventArgs
{
    public bool? DialogResult { get; } = dialogResult;
}
