using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.Services;
using GuildedChatExporter.Gui.Utils;

namespace GuildedChatExporter.Gui.ViewModels.Dialogs;

public partial class SettingsViewModel : DialogViewModelBase
{
    private readonly SettingsService _settingsService;

    [ObservableProperty]
    private ThemeVariant _selectedTheme;

    [ObservableProperty]
    private bool _isAutoUpdateEnabled;

    [ObservableProperty]
    private bool _isTokenPersisted;

    [ObservableProperty]
    private string? _selectedLocale;

    [ObservableProperty]
    private bool _isUtcNormalizationEnabled;

    [ObservableProperty]
    private int _parallelLimit;

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;

        SelectedTheme = settingsService.Theme;
        IsAutoUpdateEnabled = settingsService.IsAutoUpdateEnabled;
        IsTokenPersisted = settingsService.IsTokenPersisted;
        SelectedLocale = settingsService.Locale;
        IsUtcNormalizationEnabled = settingsService.IsUtcNormalizationEnabled;
        ParallelLimit = settingsService.ParallelLimit;
    }

    public IReadOnlyList<ThemeVariant> AvailableThemes { get; } = Enum.GetValues<ThemeVariant>();

    public IReadOnlyList<string> AvailableLocales { get; } =
        CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .OrderBy(c => c.DisplayName)
            .Select(c => c.Name)
            .Prepend("")
            .ToArray();

    [RelayCommand]
    private void Confirm()
    {
        _settingsService.Theme = SelectedTheme;
        _settingsService.IsAutoUpdateEnabled = IsAutoUpdateEnabled;
        _settingsService.IsTokenPersisted = IsTokenPersisted;
        _settingsService.Locale = SelectedLocale;
        _settingsService.IsUtcNormalizationEnabled = IsUtcNormalizationEnabled;
        _settingsService.ParallelLimit = ParallelLimit;

        Close(true);
    }

    [RelayCommand]
    private void Cancel() => Close(false);
}
