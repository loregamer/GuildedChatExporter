using System;
using System.Collections.Generic;

namespace GuildedChatExporter.Core.Guilded.Data;

public class Reaction
{
    public string Emoji { get; }

    public int Count { get; }

    public IReadOnlyList<string> Users { get; }

    public Reaction(string emoji, int count, IReadOnlyList<string>? users = null)
    {
        Emoji = emoji;
        Count = count;
        Users = users ?? Array.Empty<string>();
    }
}
