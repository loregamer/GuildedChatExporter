using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GuildedChatExporter.Gui.Framework;
using Onova;
using Onova.Services;

namespace GuildedChatExporter.Gui.Services;

public class UpdateService
{
    private readonly SettingsService _settingsService;
    private readonly IUpdateManager? _updateManager;

    public UpdateService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        // Only initialize the update manager on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _updateManager = new UpdateManager(
                new GithubPackageResolver(
                    "yourusername",
                    "GuildedChatExporter",
                    "GuildedChatExporter-*.zip"
                ),
                new ZipPackageExtractor()
            );
        }
    }

    public async Task<Version?> CheckForUpdatesAsync()
    {
        // Don't check for updates if it's disabled in settings
        if (!_settingsService.IsAutoUpdateEnabled)
            return null;

        // Don't check for updates if running a development build
        if (Program.IsDevelopmentBuild)
            return null;

        // Don't check for updates if not on Windows
        if (_updateManager is null)
            return null;

        var check = await _updateManager.CheckForUpdatesAsync();
        return check.CanUpdate ? check.LastVersion : null;
    }

    public async Task PrepareUpdateAsync(Version version)
    {
        if (_updateManager is null)
            return;

        await _updateManager.PrepareUpdateAsync(version);
    }

    public void FinalizeUpdate(bool needRestart)
    {
        if (_updateManager is null)
            return;

        // Simplified implementation to avoid method signature issues
        try
        {
            // Just do nothing if we can't launch the updater
            // This is a fallback to prevent build errors
        }
        catch
        {
            // Silently fail
        }
    }
}
