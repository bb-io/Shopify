using Apps.Shopify.HtmlConversion;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Models;

public static class PrimaryLocaleValue
{
    public static bool IsEmpty(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(value);
        if (!value.TrimStart().StartsWith('{'))
            return string.IsNullOrWhiteSpace(System.Net.WebUtility.HtmlDecode(doc.DocumentNode.InnerText)) &&
                !doc.DocumentNode.Descendants().Any(x => x.Name is "img" or "video" or "iframe" or "audio");
        try
        {
            var root = JObject.Parse(value);
            if (root.Value<string>("type") != "root") return false;
            return !root.DescendantsAndSelf().OfType<JObject>()
                .Where(x => x.Value<string>("type") == "text").Any(x => !string.IsNullOrWhiteSpace(x.Value<string>("value")));
        }
        catch (JsonReaderException) { return false; }
    }

    // Only complete scalar values and supported rich text are safe. Component/list/reference
    // keys are deliberately unsupported until their export contract can be verified.
    public static bool TryConvert(string type, string value, out string result)
    {
        result = value;
        if (type is "single_line_text_field" or "multi_line_text_field") return true;
        if (type != "rich_text_field") return false;
        try
        {
            var root = JObject.Parse(value);
            if (root.Value<string>("type") != "root" || root["children"] is not JArray || !RichTextConverter.TryToHtml(value, out _))
                throw new PluginMisconfigurationException("Invalid or unsupported Shopify rich-text JSON");
            result = root.ToString(Formatting.None);
            return true;
        }
        catch (JsonReaderException) { throw new PluginMisconfigurationException("Rich-text fields require a rich-text export; raw HTML is unsafe. Preserve the format attribute from Download content."); }
    }
}
