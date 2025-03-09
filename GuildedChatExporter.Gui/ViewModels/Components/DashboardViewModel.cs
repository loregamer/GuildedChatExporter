using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gress;
using Gress.Completable;
using GuildedChatExporter.Core.Exceptions;
using GuildedChatExporter.Core.Exporting;
using GuildedChatExporter.Core.Guilded;
using GuildedChatExporter.Core.Guilded.Data;
using GuildedChatExporter.Core.Utils.Extensions;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.Services;
using GuildedChatExporter.Gui.Utils;
using GuildedChatExporter.Gui.Utils.Extensions;

namespace GuildedChatExporter.Gui.ViewModels.Components;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ViewModelManager _viewModelManager;
    private readonly SnackbarManager _snackbarManager;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    private readonly DisposableCollector _eventRoot = new();
    private readonly AutoResetProgressMuxer _progressMuxer;

    private GuildedClient? _guilded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProgressIndeterminate))]
    [NotifyCanExecuteChangedFor(nameof(PullChannelsCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PullChannelsCommand))]
    private string? _cookie;

    [ObservableProperty]
    private IReadOnlyList<Channel>? _availableChannels;

    public DashboardViewModel(
        ViewModelManager viewModelManager,
        DialogManager dialogManager,
        SnackbarManager snackbarManager,
        SettingsService settingsService
    )
    {
        _viewModelManager = viewModelManager;
        _dialogManager = dialogManager;
        _snackbarManager = snackbarManager;
        _settingsService = settingsService;

        _progressMuxer = Progress.CreateMuxer().WithAutoReset();

        _eventRoot.Add(
            Progress.WatchProperty(
                o => o.Current,
                () => OnPropertyChanged(nameof(IsProgressIndeterminate))
            )
        );

        _eventRoot.Add(
            SelectedChannels.WatchProperty(
                o => o.Count,
                () => ExportCommand.NotifyCanExecuteChanged()
            )
        );
    }

    public ProgressContainer<Percentage> Progress { get; } = new();

    public bool IsProgressIndeterminate => IsBusy && Progress.Current.Fraction is <= 0 or >= 1;

    public ObservableCollection<Channel> SelectedChannels { get; } = [];

    [RelayCommand]
    private void Initialize()
    {
        if (!string.IsNullOrWhiteSpace(_settingsService.LastCookie))
            Cookie = _settingsService.LastCookie;
    }

    [RelayCommand]
    private async Task ShowSettingsAsync() =>
        await _dialogManager.ShowDialogAsync(_viewModelManager.CreateSettingsViewModel());

    [RelayCommand]
    private void ShowHelp() => ProcessEx.StartShellExecute(Program.ProjectDocumentationUrl);

    private bool CanPullChannels() => !IsBusy && !string.IsNullOrWhiteSpace(Cookie);

    [RelayCommand(CanExecute = nameof(CanPullChannels))]
    private async Task PullChannelsAsync()
    {
        IsBusy = true;
        var progress = _progressMuxer.CreateInput();

        try
        {
            var cookie = Cookie?.Trim('"', ' ');
            if (string.IsNullOrWhiteSpace(cookie))
                return;

            AvailableChannels = null;
            SelectedChannels.Clear();

            _guilded = new GuildedClient(cookie);
            _settingsService.LastCookie = cookie;

            var channels = new List<Channel>();

            // Direct message channels
            await foreach (var channel in _guilded.GetDirectChannelsAsync())
                channels.Add(channel);

            AvailableChannels = channels.OrderBy(c => c.Name).ToArray();
        }
        catch (GuildedChatExporterException ex) when (!ex.IsFatal)
        {
            _snackbarManager.Notify(ex.Message.TrimEnd('.'));
        }
        catch (Exception ex)
        {
            var dialog = _viewModelManager.CreateMessageBoxViewModel(
                "Error pulling channels",
                ex.ToString()
            );

            await _dialogManager.ShowDialogAsync(dialog);
        }
        finally
        {
            progress.ReportCompletion();
            IsBusy = false;
        }
    }

    private bool CanExport() => !IsBusy && _guilded is not null && SelectedChannels.Any();

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportAsync()
    {
        IsBusy = true;

        try
        {
            if (_guilded is null || !SelectedChannels.Any())
                return;

            var dialog = _viewModelManager.CreateExportSetupViewModel(SelectedChannels.ToArray());

            if (await _dialogManager.ShowDialogAsync(dialog) != true)
                return;

            var exporter = new ChannelExporter(_guilded);

            var channelProgressPairs = dialog
                .Channels!.Select(c => new { Channel = c, Progress = _progressMuxer.CreateInput() })
                .ToArray();

            var successfulExportCount = 0;

            await Parallel.ForEachAsync(
                channelProgressPairs,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(1, _settingsService.ParallelLimit),
                },
                async (pair, cancellationToken) =>
                {
                    var channel = pair.Channel;
                    var progress = pair.Progress;

                    try
                    {
                        var request = new ExportRequest(
                            channel,
                            dialog.OutputPath!,
                            dialog.AssetsDirPath,
                            dialog.SelectedFormat,
                            dialog.ShouldFormatMarkdown,
                            dialog.ShouldDownloadAssets,
                            dialog.ShouldReuseAssets,
                            _settingsService.Locale,
                            _settingsService.IsUtcNormalizationEnabled
                        );

                        await exporter.ExportChannelAsync(request, progress, cancellationToken);

                        Interlocked.Increment(ref successfulExportCount);
                    }
                    catch (GuildedChatExporterException ex) when (!ex.IsFatal)
                    {
                        _snackbarManager.Notify(ex.Message.TrimEnd('.'));
                    }
                    finally
                    {
                        progress.ReportCompletion();
                    }
                }
            );

            // Notify of the overall completion
            if (successfulExportCount > 0)
            {
                _snackbarManager.Notify(
                    $"Successfully exported {successfulExportCount} channel(s)"
                );
            }
        }
        catch (Exception ex)
        {
            var dialog = _viewModelManager.CreateMessageBoxViewModel(
                "Error exporting channel(s)",
                ex.ToString()
            );

            await _dialogManager.ShowDialogAsync(dialog);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenGuilded() => ProcessEx.StartShellExecute("https://www.guilded.gg/");

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _eventRoot.Dispose();
        }

        base.Dispose(disposing);
    }
}
