using System.Text.RegularExpressions;
using System.Web;

namespace Apps.Shopify.HtmlConversion.Constants;

// Liquid = a custom template language created by Shopify. Used in email templates in our case
public static class LiquidPlaceholder
{
    private const string CodeAttr = "data-code";
    private const string LiquidTag = @"(?:\{%.*?%\}|\{\{.*?\}\})";
    
    private static readonly Regex PlaceholderRegex = new($@"<span {CodeAttr}=""([^""]*)""></span>", RegexOptions.Compiled);

    private static readonly Regex LiquidRegex = new(
        $@"(?:^\s*)?{LiquidTag}(?:\s*{LiquidTag})*(?:\s*$)?",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public static string Lock(string text)
    {
        return LiquidRegex.Replace(text, m => $"<span {CodeAttr}=\"{HttpUtility.HtmlEncode(m.Value)}\"></span>");
    }
    
    public static string Unlock(string html)
    {
        return PlaceholderRegex.Replace(html, m => HttpUtility.HtmlDecode(m.Groups[1].Value));
    }
}