using System;
using System.Runtime.InteropServices;

namespace GuildedChatExporter.Core.Utils;

public static class Docker
{
    public static bool IsRunningInContainer { get; } = IsRunningInContainerInternal();

    private static bool IsRunningInContainerInternal()
    {
        // Not in Docker if not on Linux
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return false;

        try
        {
            // Docker creates a .dockerenv file at the root of the container
            return System.IO.File.Exists("/.dockerenv");
        }
        catch
        {
            return false;
        }
    }
}
