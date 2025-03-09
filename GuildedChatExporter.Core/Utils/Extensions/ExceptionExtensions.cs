using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using GuildedChatExporter.Core.Exceptions;

namespace GuildedChatExporter.Core.Utils.Extensions;

public static class ExceptionExtensions
{
    public static GuildedChatExporterException ToGuildedChatExporterException(
        this Exception exception
    ) =>
        exception is GuildedChatExporterException guildedException ? guildedException
        : exception is HttpRequestException httpRequestException
            ? httpRequestException.ToGuildedChatExporterException()
        : new GuildedChatExporterException($"Unknown error: {exception.Message}", exception, true);

    private static GuildedChatExporterException ToGuildedChatExporterException(
        this HttpRequestException exception
    )
    {
        var message = exception.Message;
        var statusCode = exception.StatusCode;

        // Try to extract a more specific error message from the response
        if (exception.Data.Contains("Response"))
        {
            var responseJson = exception.Data["Response"] as string;
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                try
                {
                    var response = JsonSerializer.Deserialize<JsonElement>(responseJson);
                    if (
                        response.TryGetProperty("message", out var messageElement)
                        && messageElement.ValueKind == JsonValueKind.String
                    )
                    {
                        message = messageElement.GetString() ?? message;
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors
                }
            }
        }

        // Determine if the error is fatal based on the status code
        var isFatal = statusCode switch
        {
            HttpStatusCode.Unauthorized => false,
            HttpStatusCode.Forbidden => false,
            HttpStatusCode.NotFound => false,
            _ => true,
        };

        return new GuildedChatExporterException($"Request failed: {message}", exception, isFatal);
    }
}
