using System;
using System.Text.Json.Serialization;

namespace GuildedChatExporter.Core.Guilded;

public readonly struct GuildedId : IEquatable<GuildedId>, IComparable<GuildedId>
{
    [JsonInclude]
    public string Value { get; }

    public GuildedId(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool Equals(GuildedId other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is GuildedId other && Equals(other);

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    public int CompareTo(GuildedId other) => StringComparer.Ordinal.Compare(Value, other.Value);

    public override string ToString() => Value;

    public static bool operator ==(GuildedId left, GuildedId right) => left.Equals(right);

    public static bool operator !=(GuildedId left, GuildedId right) => !left.Equals(right);

    public static implicit operator string(GuildedId id) => id.Value;
}
