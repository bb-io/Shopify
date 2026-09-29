using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Apps.Shopify.HtmlConversion.Models;

// Liquid = a custom template language created by Shopify. Used in email templates in our case
public static class LiquidPlaceholder
{
    private const string CodeAttr = "data-code";
    private const string LiquidTag = @"(?:\{%.*?%\}|\{\{.*?\}\})";
    private const string TagAttrPrefix = "data-liquid-";

    private static readonly Regex TagPlaceholderRegex = new(
        $@"(?<before>\s?){TagAttrPrefix}\d+(?:-(?<flags>[ba]+))?=""(?<code>[^""]*)""(?<after>\s?)",
        RegexOptions.Compiled);
    private static readonly Regex PlaceholderRegex = new($@"<span {CodeAttr}=""([^""]*)""></span>", RegexOptions.Compiled);
    private static readonly Regex LiquidRegex = new($@"(?:^\s*)?{LiquidTag}(?:\s*{LiquidTag})*(?:\s*$)?", RegexOptions.Compiled | RegexOptions.Singleline);

    public static string Lock(string text)
    {
        return LiquidRegex.Replace(text, m => $"<span {CodeAttr}=\"{HttpUtility.HtmlEncode(m.Value)}\"></span>");
    }
    
    public static string LockTags(string html)
    {
        var result = new StringBuilder(html.Length);
        bool inTag = false;
        char? quote = null;
        int counter = 0;

        for (int i = 0; i < html.Length; i++)
        {
            char c = html[i];

            if (inTag && quote == null && c == '{' && i + 1 < html.Length && html[i + 1] is '%' or '{')
            {
                int endIndex = html.IndexOf(html[i + 1] == '%' ? "%}" : "}}", i + 2, StringComparison.Ordinal);
                if (endIndex >= 0)
                {
                    result.Append(ToTagPlaceholder(html, i, endIndex + 2, ++counter));
                    i = endIndex + 1;
                    continue;
                }
            }

            result.Append(c);

            if (!inTag)
                inTag = c == '<' && i + 1 < html.Length && char.IsLetter(html[i + 1]);
            else if (quote != null)
                quote = c == quote ? null : quote;
            else if (c is '"' or '\'')
                quote = c;
            else if (c == '>')
                inTag = false;
        }

        return result.ToString();
    }
    
    public static string Unlock(string html)
    {
        html = PlaceholderRegex.Replace(html, m => HttpUtility.HtmlDecode(m.Groups[1].Value));

        return TagPlaceholderRegex.Replace(html, m =>
        {
            string flags = m.Groups["flags"].Value;
            string before = flags.Contains('b') ? string.Empty : m.Groups["before"].Value;
            string after = flags.Contains('a') ? string.Empty : m.Groups["after"].Value;

            return before + HttpUtility.HtmlDecode(m.Groups["code"].Value) + after;
        });
    }
    
    private static string ToTagPlaceholder(string html, int start, int end, int number)
    {
        bool addBefore = !char.IsWhiteSpace(html[start - 1]);
        bool addAfter = end < html.Length && !char.IsWhiteSpace(html[end]) && html[end] is not ('>' or '/');
        string flags = (addBefore ? "b" : "") + (addAfter ? "a" : "");
        string name = $"{TagAttrPrefix}{number}{(flags.Length > 0 ? "-" + flags : "")}";

        return (addBefore ? " " : "") + $"{name}=\"{HttpUtility.HtmlEncode(html[start..end])}\"" + (addAfter ? " " : "");
    }
}