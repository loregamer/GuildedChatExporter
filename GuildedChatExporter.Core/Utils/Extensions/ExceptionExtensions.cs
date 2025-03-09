using System;
using System.Linq;

namespace GuildedChatExporter.Core.Utils.Extensions;

public static class ExceptionExtensions
{
    public static bool IsCausedBy<T>(this Exception exception)
        where T : Exception
    {
        var inner = exception;
        while (inner != null)
        {
            if (inner is T)
                return true;

            inner = inner.InnerException;
        }

        return false;
    }

    public static string JoinMessages(this Exception exception)
    {
        var inner = exception;
        var messages = new string[0];

        while (inner != null)
        {
            messages = messages.Append(inner.Message).ToArray();
            inner = inner.InnerException;
        }

        return string.Join(" -> ", messages);
    }
}
