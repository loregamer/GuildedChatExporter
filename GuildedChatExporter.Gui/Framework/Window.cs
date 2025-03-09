using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace GuildedChatExporter.Gui.Framework;

public class Window : Avalonia.Controls.Window
{
    public Window()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ShowInTaskbar = true;
        CanResize = true;
        WindowState = WindowState.Normal;
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
        SystemDecorations = SystemDecorations.Full;
    }
}
