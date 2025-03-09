using System;
using GuildedChatExporter.Core.Guilded.Data;
using GuildedChatExporter.Gui.ViewModels;
using GuildedChatExporter.Gui.ViewModels.Components;
using GuildedChatExporter.Gui.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace GuildedChatExporter.Gui.Framework;

public class ViewModelManager
{
    private readonly IServiceProvider _services;

    public ViewModelManager(IServiceProvider services) => _services = services;

    public MainViewModel CreateMainViewModel() => _services.GetRequiredService<MainViewModel>();

    public DashboardViewModel CreateDashboardViewModel() =>
        _services.GetRequiredService<DashboardViewModel>();

    public ExportSetupViewModel CreateExportSetupViewModel(Channel[] channels)
    {
        var viewModel = _services.GetRequiredService<ExportSetupViewModel>();
        viewModel.Channels = channels;
        viewModel.InitializeCommand.Execute(null);
        return viewModel;
    }

    public MessageBoxViewModel CreateMessageBoxViewModel(
        string title,
        string message,
        string? primaryButtonText = null,
        string secondaryButtonText = "CLOSE"
    ) => new(title, message, primaryButtonText, secondaryButtonText);

    public SettingsViewModel CreateSettingsViewModel() =>
        _services.GetRequiredService<SettingsViewModel>();
}
