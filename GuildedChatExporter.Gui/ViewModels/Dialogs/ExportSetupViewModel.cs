using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GuildedChatExporter.Core.Exporting;
using GuildedChatExporter.Core.Guilded.Data;
using GuildedChatExporter.Core.Utils.Extensions;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.Services;

namespace GuildedChatExporter.Gui.ViewModels.Dialogs;

public partial class ExportSetupViewModel(
    DialogManager dialogManager,
    SettingsService settingsService
) : DialogViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSingleChannel))]
    private IReadOnlyList<Channel>? _channels;

    [ObservableProperty]
    private string? _outputPath;

    [ObservableProperty]
    private ExportFormat _selectedFormat;

    [ObservableProperty]
    private bool _shouldFormatMarkdown;

    [ObservableProperty]
    private bool _shouldDownloadAssets;

    [ObservableProperty]
    private bool _shouldReuseAssets;

    [ObservableProperty]
    private string? _assetsDirPath;

    [ObservableProperty]
    private bool _isAdvancedSectionDisplayed;

    public bool IsSingleChannel => Channels?.Count == 1;

    public IReadOnlyList<ExportFormat> AvailableFormats { get; } = Enum.GetValues<ExportFormat>();

    [RelayCommand]
    private void Initialize()
    {
        // Persist preferences
        SelectedFormat = settingsService.LastExportFormat;
        ShouldFormatMarkdown = settingsService.LastShouldFormatMarkdown;
        ShouldDownloadAssets = settingsService.LastShouldDownloadAssets;
        ShouldReuseAssets = settingsService.LastShouldReuseAssets;
        AssetsDirPath = settingsService.LastAssetsDirPath;

        // Show the "advanced options" section by default if any
        // of the advanced options are set to non-default values.
        IsAdvancedSectionDisplayed =
            ShouldDownloadAssets || ShouldReuseAssets || !string.IsNullOrWhiteSpace(AssetsDirPath);
    }

    [RelayCommand]
    private async Task ShowOutputPathPromptAsync()
    {
        if (IsSingleChannel)
        {
            var defaultFileName = $"{Channels!.Single().DisplayName} [{Channels!.Single().Id}]";
            var extension = SelectedFormat.GetFileExtension();

            var path = await dialogManager.PromptSaveFilePathAsync(
                [
                    new FilePickerFileType($"{extension.ToUpperInvariant()} file")
                    {
                        Patterns = [$"*.{extension}"],
                    },
                ],
                defaultFileName
            );

            if (!string.IsNullOrWhiteSpace(path))
                OutputPath = path;
        }
        else
        {
            var path = await dialogManager.PromptDirectoryPathAsync();
            if (!string.IsNullOrWhiteSpace(path))
                OutputPath = path;
        }
    }

    [RelayCommand]
    private async Task ShowAssetsDirPathPromptAsync()
    {
        var path = await dialogManager.PromptDirectoryPathAsync();
        if (!string.IsNullOrWhiteSpace(path))
            AssetsDirPath = path;
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        // Prompt the output path if it hasn't been set yet
        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            await ShowOutputPathPromptAsync();

            // If the output path is still not set, cancel the export
            if (string.IsNullOrWhiteSpace(OutputPath))
                return;
        }

        // Persist preferences
        settingsService.LastExportFormat = SelectedFormat;
        settingsService.LastShouldFormatMarkdown = ShouldFormatMarkdown;
        settingsService.LastShouldDownloadAssets = ShouldDownloadAssets;
        settingsService.LastShouldReuseAssets = ShouldReuseAssets;
        settingsService.LastAssetsDirPath = AssetsDirPath;

        Close(true);
    }
}
