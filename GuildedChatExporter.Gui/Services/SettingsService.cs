using System;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using GuildedChatExporter.Core.Exporting;
using GuildedChatExporter.Gui.Framework;

namespace GuildedChatExporter.Gui.Services;

// Can't use [ObservableProperty] here because System.Text.Json's source generator doesn't see
// the generated properties.
[INotifyPropertyChanged]
public partial class SettingsService()
    : SettingsBase(
        Path.Combine(AppContext.BaseDirectory, "Settings.dat"),
        SerializerContext.Default
    )
{
    private ThemeVariant _theme;
    public ThemeVariant Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    private bool _isAutoUpdateEnabled = true;
    public bool IsAutoUpdateEnabled
    {
        get => _isAutoUpdateEnabled;
        set => SetProperty(ref _isAutoUpdateEnabled, value);
    }

    private bool _isTokenPersisted = true;
    public bool IsTokenPersisted
    {
        get => _isTokenPersisted;
        set => SetProperty(ref _isTokenPersisted, value);
    }

    private string? _locale;
    public string? Locale
    {
        get => _locale;
        set => SetProperty(ref _locale, value);
    }

    private bool _isUtcNormalizationEnabled;
    public bool IsUtcNormalizationEnabled
    {
        get => _isUtcNormalizationEnabled;
        set => SetProperty(ref _isUtcNormalizationEnabled, value);
    }

    private int _parallelLimit = 1;
    public int ParallelLimit
    {
        get => _parallelLimit;
        set => SetProperty(ref _parallelLimit, value);
    }

    private string? _lastCookie;
    public string? LastCookie
    {
        get => _lastCookie;
        set => SetProperty(ref _lastCookie, value);
    }

    private ExportFormat _lastExportFormat = ExportFormat.HtmlDark;
    public ExportFormat LastExportFormat
    {
        get => _lastExportFormat;
        set => SetProperty(ref _lastExportFormat, value);
    }

    private bool _lastShouldFormatMarkdown = true;
    public bool LastShouldFormatMarkdown
    {
        get => _lastShouldFormatMarkdown;
        set => SetProperty(ref _lastShouldFormatMarkdown, value);
    }

    private bool _lastShouldDownloadAssets;
    public bool LastShouldDownloadAssets
    {
        get => _lastShouldDownloadAssets;
        set => SetProperty(ref _lastShouldDownloadAssets, value);
    }

    private bool _lastShouldReuseAssets;
    public bool LastShouldReuseAssets
    {
        get => _lastShouldReuseAssets;
        set => SetProperty(ref _lastShouldReuseAssets, value);
    }

    private string? _lastAssetsDirPath;
    public string? LastAssetsDirPath
    {
        get => _lastAssetsDirPath;
        set => SetProperty(ref _lastAssetsDirPath, value);
    }

    public override void Save()
    {
        // Clear the token if it's not supposed to be persisted
        var lastCookie = LastCookie;
        if (!IsTokenPersisted)
            LastCookie = null;

        base.Save();

        LastCookie = lastCookie;
    }
}

public partial class SettingsService
{
    [JsonSerializable(typeof(SettingsService))]
    private partial class SerializerContext : JsonSerializerContext;
}
