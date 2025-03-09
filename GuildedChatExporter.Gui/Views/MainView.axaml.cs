using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using GuildedChatExporter.Gui.Framework;
using GuildedChatExporter.Gui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GuildedChatExporter.Gui.Views;

public partial class MainView : Framework.Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext!;

    public MainView()
    {
        InitializeComponent();

        this.Opened += (s, e) =>
        {
            this.Activate();
            this.Focus();
            this.Show();
        };
    }

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args)
    {
        var app = (App)App.Current!;
        var dialogManager = app.Services.GetRequiredService<DialogManager>();
        dialogManager.RegisterHost(DialogHost);
        ViewModel.InitializeCommand.Execute(null);
    }
}
