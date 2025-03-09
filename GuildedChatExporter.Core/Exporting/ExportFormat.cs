using System;

namespace GuildedChatExporter.Core.Exporting;

public enum ExportFormat
{
    HtmlDark,
    HtmlLight,
    PlainText,
    Json,
    Csv,
}

public static class ExportFormatExtensions
{
    public static string GetFileExtension(this ExportFormat format) =>
        format switch
        {
            ExportFormat.HtmlDark => ".html",
            ExportFormat.HtmlLight => ".html",
            ExportFormat.PlainText => ".txt",
            ExportFormat.Json => ".json",
            ExportFormat.Csv => ".csv",
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };

    public static string GetDisplayName(this ExportFormat format) =>
        format switch
        {
            ExportFormat.HtmlDark => "HTML (Dark)",
            ExportFormat.HtmlLight => "HTML (Light)",
            ExportFormat.PlainText => "Plain Text",
            ExportFormat.Json => "JSON",
            ExportFormat.Csv => "CSV",
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
}
