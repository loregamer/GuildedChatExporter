using System;
using System.Net;
using System.Net.Http;

namespace GuildedChatExporter.Core.Utils;

public static class Http
{
    private static readonly Lazy<HttpClient> DefaultClientLazy = new(() => CreateClient());

    public static HttpClient DefaultClient => DefaultClientLazy.Value;

    public static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler, true);

        // Set default headers
        client.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
        );

        return client;
    }
}
