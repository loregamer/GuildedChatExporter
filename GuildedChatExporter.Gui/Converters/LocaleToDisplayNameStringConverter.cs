using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace GuildedChatExporter.Gui.Converters;

public class LocaleToDisplayNameStringConverter : IValueConverter
{
    public static LocaleToDisplayNameStringConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string locale)
            return string.Empty;

        if (string.IsNullOrWhiteSpace(locale))
            return "System default";

        try
        {
            return new CultureInfo(locale).DisplayName;
        }
        catch
        {
            return locale;
        }
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
