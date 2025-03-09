using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GuildedChatExporter.Gui.Utils;

public static class ProcessEx
{
    public static void StartShellExecute(string path)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Use explorer.exe on Windows
                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Use xdg-open on Linux
                Process.Start("xdg-open", path);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Use open on macOS
                Process.Start("open", path);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to open URL: {ex.Message}");
        }
    }
}
