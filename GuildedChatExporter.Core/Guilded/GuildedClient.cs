using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using GuildedChatExporter.Core.Exceptions;
using GuildedChatExporter.Core.Guilded.Data;
using GuildedChatExporter.Core.Utils;
using GuildedChatExporter.Core.Utils.Extensions;

namespace GuildedChatExporter.Core.Guilded;

public class GuildedClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _cookies;

    public GuildedClient(string cookies)
    {
        _httpClient = Http.CreateClient();
        _cookies = cookies;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Cookie", _cookies);
        return request;
    }

    public async Task<string> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        // This is a placeholder implementation
        // In a real implementation, we would make a request to the Guilded API to get the user ID
        // For now, we'll just return a dummy value
        await Task.Delay(100, cancellationToken);
        return "user-123";
    }

    public async Task<Channel> GetChannelAsync(
        GuildedId channelId,
        CancellationToken cancellationToken = default
    )
    {
        // This is a placeholder implementation
        // In a real implementation, we would make a request to the Guilded API to get the channel
        // For now, we'll just return a dummy channel
        await Task.Delay(100, cancellationToken);
        return new Channel(
            channelId,
            "Channel Name",
            "chat",
            "Channel Topic",
            DateTimeOffset.Now.AddDays(-30),
            "Creator Name",
            DateTimeOffset.Now
        );
    }

    public async IAsyncEnumerable<Channel> GetDirectChannelsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        // This is a placeholder implementation
        // In a real implementation, we would make a request to the Guilded API to get the direct channels
        // For now, we'll just return a dummy channel
        await Task.Delay(100, cancellationToken);
        yield return new Channel(
            new GuildedId("channel-123"),
            "Direct Message Channel",
            "dm",
            null,
            DateTimeOffset.Now.AddDays(-30),
            "Creator Name",
            DateTimeOffset.Now
        );
    }

    public async IAsyncEnumerable<Message> GetMessagesAsync(
        GuildedId channelId,
        IProgress<double>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        // This is a placeholder implementation
        // In a real implementation, we would make a request to the Guilded API to get the messages
        // For now, we'll just return a dummy message
        await Task.Delay(100, cancellationToken);
        progress?.Report(0.5);
        yield return new Message(
            new GuildedId("message-123"),
            channelId,
            "Hello, world!",
            DateTimeOffset.Now.AddDays(-1),
            "User Name",
            null,
            null,
            new[] { new Reaction("👍", 1, new[] { "User Name" }) }
        );
        progress?.Report(1.0);
    }

    public void Dispose() => _httpClient.Dispose();
}
