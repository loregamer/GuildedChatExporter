using GuildedChatExporter.Core.Guilded.Data.Common;

namespace GuildedChatExporter.Core.Guilded.Data;

public class User : IHasId
{
    public GuildedId Id { get; }

    public string Name { get; }

    public string? AvatarUrl { get; }

    public User(GuildedId id, string name, string? avatarUrl = null)
    {
        Id = id;
        Name = name;
        AvatarUrl = avatarUrl;
    }
}
