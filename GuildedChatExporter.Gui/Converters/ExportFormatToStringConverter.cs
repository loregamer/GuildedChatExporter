using System;
using System.Globalization;
using Avalonia.Data.Converters;
using GuildedChatExporter.Core.Exporting;

namespace GuildedChatExporter.Gui.Converters;

public class ExportFormatToStringConverter : IValueConverter
{
    public static ExportFormatToStringConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ExportFormat format)
            return format.GetDisplayName();

        return string.Empty;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
