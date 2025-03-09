using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.Services;
using GuildedChatExporter.Gui.Utils;
using GuildedChatExporter.Gui.Utils.Extensions;
using GuildedChatExporter.Gui.ViewModels.Components;

namespace GuildedChatExporter.Gui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DialogManager _dialogManager;
    private readonly SnackbarManager _snackbarManager;
    private readonly SettingsService _settingsService;
    private readonly UpdateService _updateService;

    public MainViewModel(
        ViewModelManager viewModelManager,
        DialogManager dialogManager,
        SnackbarManager snackbarManager,
        SettingsService settingsService,
        UpdateService updateService
    )
    {
        _dialogManager = dialogManager;
        _snackbarManager = snackbarManager;
        _settingsService = settingsService;
        _updateService = updateService;

        Dashboard = viewModelManager.CreateDashboardViewModel();
    }

    public string Title { get; } = $"{Program.Name} v{Program.VersionString}";

    public DashboardViewModel Dashboard { get; }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            var updateVersion = await _updateService.CheckForUpdatesAsync();
            if (updateVersion is null)
                return;

            _snackbarManager.Notify($"Downloading update to {Program.Name} v{updateVersion}...");
            await _updateService.PrepareUpdateAsync(updateVersion);

            _snackbarManager.Notify(
                "Update has been downloaded and will be installed when you exit",
                "INSTALL NOW",
                () =>
                {
                    _updateService.FinalizeUpdate(true);

                    // Exit the application
                    Environment.Exit(2);
                }
            );
        }
        catch
        {
            // Failure to update shouldn't crash the application
            _snackbarManager.Notify("Failed to perform application update");
        }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await CheckForUpdatesAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Save settings
            _settingsService.Save();

            // Finalize pending updates
            _updateService.FinalizeUpdate(false);
        }

        base.Dispose(disposing);
    }
}
