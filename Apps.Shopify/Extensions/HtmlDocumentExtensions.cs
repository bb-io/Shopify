using HtmlAgilityPack;

namespace Apps.Shopify.Extensions;

public static class HtmlDocumentExtensions
{
    public static HtmlDocument AddMeta(this HtmlDocument htmlDoc, HtmlNode headNode, string metaName, string? metaValue)
    {
        if (string.IsNullOrWhiteSpace(metaValue))
            return htmlDoc;
        
        var contentTypeMetaNode = htmlDoc.CreateElement("meta");
        contentTypeMetaNode.SetAttributeValue("name", metaName);
        contentTypeMetaNode.SetAttributeValue("content", metaValue);
        headNode.AppendChild(contentTypeMetaNode);

        return htmlDoc;
    }
    
    public static string? GetMeta(this HtmlDocument htmlDoc, string metaName)
    {
        return htmlDoc.DocumentNode
            .SelectSingleNode($"//meta[@name='{metaName}']")?
            .GetAttributeValue("content", string.Empty);
    }
}