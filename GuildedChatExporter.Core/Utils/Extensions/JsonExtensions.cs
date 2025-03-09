using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildedChatExporter.Core.Utils.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T? ParseJson<T>(this string json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);

    public static string FormatJson<T>(this T obj, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(obj, options ?? DefaultOptions);

    public static JsonElement? GetPropertyOrNull(this JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) ? property : null;

    public static string? GetStringOrNull(this JsonElement element) =>
        element.ValueKind == JsonValueKind.String ? element.GetString() : null;

    public static int? GetInt32OrNull(this JsonElement element) =>
        element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var value)
            ? value
            : null;

    public static bool? GetBooleanOrNull(this JsonElement element) =>
        element.ValueKind == JsonValueKind.True
            ? true
            : element.ValueKind == JsonValueKind.False
                ? false
                : null;

    public static DateTimeOffset? GetDateTimeOffsetOrNull(this JsonElement element) =>
        element.ValueKind == JsonValueKind.String
        && DateTimeOffset.TryParse(element.GetString(), out var value)
            ? value
            : null;
}
