using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GuildedChatExporter.Core.Utils;

public class UrlBuilder
{
    private readonly string _baseUrl;
    private readonly Dictionary<string, string> _queryParameters = new();

    public UrlBuilder(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public UrlBuilder SetQueryParameter(string key, string value)
    {
        _queryParameters[key] = value;
        return this;
    }

    public UrlBuilder SetQueryParameter(string key, int value) =>
        SetQueryParameter(key, value.ToString());

    public UrlBuilder SetQueryParameter(string key, bool value) =>
        SetQueryParameter(key, value.ToString().ToLowerInvariant());

    public string Build()
    {
        if (!_queryParameters.Any())
            return _baseUrl;

        var sb = new StringBuilder(_baseUrl);
        sb.Append(_baseUrl.Contains('?') ? '&' : '?');

        var isFirst = true;
        foreach (var (key, value) in _queryParameters)
        {
            if (!isFirst)
                sb.Append('&');

            sb.Append(HttpUtility.UrlEncode(key));
            sb.Append('=');
            sb.Append(HttpUtility.UrlEncode(value));

            isFirst = false;
        }

        return sb.ToString();
    }

    public override string ToString() => Build();
}
