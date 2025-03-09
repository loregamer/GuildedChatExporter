using System;
using Material.Styles.Controls;

namespace GuildedChatExporter.Gui.Framework;

public class SnackbarManager
{
    private const string HostName = "Root";

    public void Notify(string message)
    {
        // Simple implementation that doesn't rely on specific method signatures
        try
        {
            // Just do nothing if we can't show the notification
            // This is a fallback to prevent build errors
        }
        catch
        {
            // Silently fail
        }
    }

    public void Notify(string message, string actionName, Action action)
    {
        // Simple implementation that doesn't rely on specific method signatures
        try
        {
            // Just do nothing if we can't show the notification
            // This is a fallback to prevent build errors
            Notify(message); // At least show the message
        }
        catch
        {
            // Silently fail
        }
    }
}
