using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace GuildedChatExporter.Gui.Framework;

public class ViewManager
{
    private readonly IServiceProvider _services;

    public ViewManager(IServiceProvider services) => _services = services;

    public T CreateView<T>()
        where T : Control => _services.GetRequiredService<T>();
}
