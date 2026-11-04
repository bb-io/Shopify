using System.Net;
using System.Web;
using Apps.Shopify.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.HtmlConversion;

public static class RichTextConverter
{
    public const string Format = "rich-text";
    private static readonly HashSet<string> SupportedTypes = ["root", "paragraph", "heading", "list", "list-item", "link", "text"];

    public static bool TryToHtml(string? value, out string html)
    {
        html = string.Empty;
        if (string.IsNullOrWhiteSpace(value) || !value.TrimStart().StartsWith('{'))
            return false;

        JObject root;
        try
        {
            root = JObject.Parse(value);
        }
        catch (JsonReaderException)
        {
            return false;
        }

        if (root.Value<string>("type") != "root" || !IsSupported(root))
            return false;

        html = string.Concat(root.ChildNodes().Select(NodeToHtml));
        return true;
    }

    public static string ToJson(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var blocks = doc.DocumentNode.ChildNodes
            .Where(x => x is not HtmlTextNode text || !string.IsNullOrWhiteSpace(text.Text))
            .Select(BlockToJson);

        return Node("root", blocks).ToString(Formatting.None);
    }
    
    private static bool IsSupported(JToken node)
    {
        return node is JObject && SupportedTypes.Contains(node.Value<string>("type") ?? string.Empty) && node.ChildNodes().All(IsSupported);
    }

    private static string NodeToHtml(JToken node)
    {
        var inner = string.Concat(node.ChildNodes().Select(NodeToHtml));

        return node.Value<string>("type") switch
        {
            "paragraph" => $"<p>{inner}</p>",
            "heading" => $"<h{node.Value<int?>("level") ?? 1}>{inner}</h{node.Value<int?>("level") ?? 1}>",
            "list" => node.Value<string>("listType") == "ordered" ? $"<ol>{inner}</ol>" : $"<ul>{inner}</ul>",
            "list-item" => $"<li>{inner}</li>",
            "link" => $"<a{Attr("href", node["url"])}{Attr("title", node["title"])}{Attr("target", node["target"])}>{inner}</a>",
            "text" => TextToHtml(node),
            _ => inner
        };
    }

    private static string TextToHtml(JToken node)
    {
        var html = WebUtility.HtmlEncode(node.Value<string>("value") ?? string.Empty).Replace("\n", "<br>");
        if (node.Value<bool?>("italic") == true)
            html = $"<em>{html}</em>";
        if (node.Value<bool?>("bold") == true)
            html = $"<strong>{html}</strong>";

        return html;
    }

    private static string Attr(string name, JToken? value) =>
        value?.Type == JTokenType.String ? $" {name}=\"{WebUtility.HtmlEncode(value.ToString())}\"" : string.Empty;

    private static JObject BlockToJson(HtmlNode node)
    {
        switch (node.Name)
        {
            case "p":
                return Node("paragraph", InlineToJson(node.ChildNodes));
            case "h1" or "h2" or "h3" or "h4" or "h5" or "h6":
                var heading = Node("heading", InlineToJson(node.ChildNodes));
                heading["level"] = node.Name[1] - '0';
                return heading;
            case "ul" or "ol":
                var items = node.ChildNodes
                    .Where(x => x.Name == "li")
                    .Select(x => Node("list-item", InlineToJson(x.ChildNodes)));
                var list = Node("list", items);
                list["listType"] = node.Name == "ol" ? "ordered" : "unordered";
                return list;
            default:
                // Stray text or inline element on block level - wrap it in a paragraph
                return Node("paragraph", InlineToJson([node]));
        }
    }

    private static IEnumerable<JObject> InlineToJson(IEnumerable<HtmlNode> nodes, bool bold = false, bool italic = false) =>
        nodes.SelectMany(x => x switch
        {
            HtmlTextNode text => TextToJson(HttpUtility.HtmlDecode(text.Text), bold, italic),
            { Name: "br" } => TextToJson("\n", bold, italic),
            { Name: "strong" or "b" } => InlineToJson(x.ChildNodes, true, italic),
            { Name: "em" or "i" } => InlineToJson(x.ChildNodes, bold, true),
            { Name: "a" } => new[] { LinkToJson(x, bold, italic) },
            _ => InlineToJson(x.ChildNodes, bold, italic)
        });

    private static IEnumerable<JObject> TextToJson(string value, bool bold, bool italic)
    {
        if (value.Length == 0)
            yield break;

        var text = new JObject { ["type"] = "text", ["value"] = value };
        if (bold)
            text["bold"] = true;
        if (italic)
            text["italic"] = true;

        yield return text;
    }

    private static JObject LinkToJson(HtmlNode node, bool bold, bool italic)
    {
        var link = Node("link", InlineToJson(node.ChildNodes, bold, italic));
        foreach (var (attr, key) in new[] { ("href", "url"), ("title", "title"), ("target", "target") })
        {
            var value = node.Attributes[attr]?.Value;
            if (value != null)
                link[key] = HttpUtility.HtmlDecode(value);
        }

        return link;
    }

    private static JObject Node(string type, IEnumerable<JObject> children) =>
        new() { ["type"] = type, ["children"] = MergeText(children) };

    private static JArray MergeText(IEnumerable<JObject> nodes)
    {
        var result = new JArray();
        foreach (var node in nodes)
        {
            if (result.Last is JObject prev && prev.IsText() && node.IsText() && prev.HasSameFormat(node))
                prev["value"] = prev.Value<string>("value") + node.Value<string>("value");
            else
                result.Add(node);
        }

        return result;
    }
}