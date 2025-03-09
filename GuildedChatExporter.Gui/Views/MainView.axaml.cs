using System;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GuildedChatExporter.Gui.Views;

public partial class MainView : Avalonia.Controls.Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext!;

    public MainView()
    {
        InitializeComponent();
    }

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args)
    {
        // Register the dialog host
        var app = (App)App.Current!;
        var dialogManager = app.Services.GetRequiredService<DialogManager>();
        dialogManager.RegisterHost(DialogHost);

        // Initialize the view model
        ViewModel.InitializeCommand.Execute(null);
    }
}
