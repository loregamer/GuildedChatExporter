using System;
using System.Collections.Generic;
using GuildedChatExporter.Core.Guilded.Data.Common;

namespace GuildedChatExporter.Core.Guilded.Data;

public class Message : IHasId
{
    public GuildedId Id { get; }

    public GuildedId ChannelId { get; }

    public string Content { get; }

    public DateTimeOffset CreatedAt { get; }

    public string CreatedBy { get; }

    public DateTimeOffset? UpdatedAt { get; }

    public string? UpdatedBy { get; }

    public IReadOnlyList<Reaction> Reactions { get; }

    public Message(
        GuildedId id,
        GuildedId channelId,
        string content,
        DateTimeOffset createdAt,
        string createdBy,
        DateTimeOffset? updatedAt,
        string? updatedBy,
        IReadOnlyList<Reaction>? reactions = null
    )
    {
        Id = id;
        ChannelId = channelId;
        Content = content;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        Reactions = reactions ?? Array.Empty<Reaction>();
    }

    public string GetPlainTextContent() => Content;
}
