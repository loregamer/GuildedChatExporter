using System;

namespace GuildedChatExporter.Core.Exceptions;

public class GuildedChatExporterException : Exception
{
    public bool IsFatal { get; }

    public GuildedChatExporterException(string message, bool isFatal = false)
        : base(message)
    {
        IsFatal = isFatal;
    }

    public GuildedChatExporterException(string message, Exception innerException, bool isFatal = false)
        : base(message, innerException)
    {
        IsFatal = isFatal;
    }
}
