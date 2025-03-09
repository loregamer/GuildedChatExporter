using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Skia;

namespace GuildedChatExporter.Gui;

public static class Program
{
    public static string Name => "GuildedChatExporter";

    public static Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public static string VersionString => Version.ToString(3);

    public static bool IsDevelopmentBuild => Version.Revision > 0;

    public static string ProjectUrl => "https://github.com/yourusername/GuildedChatExporter";

    public static string ProjectReleasesUrl => $"{ProjectUrl}/releases";

    public static string ProjectDocumentationUrl => $"{ProjectUrl}#readme";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 });
}
