using System;
using GuildedChatExporter.Core.Guilded.Data.Common;

namespace GuildedChatExporter.Core.Guilded.Data;

public class Channel : IHasId
{
    public GuildedId Id { get; }

    public string Name { get; }

    public string Type { get; }

    public string? Topic { get; }

    public DateTimeOffset CreatedAt { get; }

    public string CreatedBy { get; }

    public DateTimeOffset UpdatedAt { get; }

    public string DisplayName => Name;

    public Channel(
        GuildedId id,
        string name,
        string type,
        string? topic,
        DateTimeOffset createdAt,
        string createdBy,
        DateTimeOffset updatedAt
    )
    {
        Id = id;
        Name = name;
        Type = type;
        Topic = topic;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
    }
}
