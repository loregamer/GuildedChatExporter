using CommunityToolkit.Mvvm.Input;
using GuildedChatExporter.Gui.Framework;

namespace GuildedChatExporter.Gui.ViewModels.Dialogs;

public partial class MessageBoxViewModel : DialogViewModelBase
{
    public string Title { get; }

    public string Message { get; }

    public string? PrimaryButtonText { get; }

    public string SecondaryButtonText { get; }

    public MessageBoxViewModel(
        string title,
        string message,
        string? primaryButtonText = null,
        string secondaryButtonText = "CLOSE"
    )
    {
        Title = title;
        Message = message;
        PrimaryButtonText = primaryButtonText;
        SecondaryButtonText = secondaryButtonText;
    }

    [RelayCommand]
    private void Confirm() => Close(true);

    [RelayCommand]
    private void Cancel() => Close(false);
}
