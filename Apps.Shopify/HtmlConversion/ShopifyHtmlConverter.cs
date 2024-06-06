using System.Web;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.TranslatableResource;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion;

public static class ShopifyHtmlConverter
{
    private const string ResourceAttr = "resource";
    private const string KeyAttr = "key";
    private const string DigestAttr = "digest";

    public static MemoryStream ToHtml(IEnumerable<ContentEntity> contentEntities)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        contentEntities.ToList().ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.InnerHtml = x.Value;
            node.SetAttributeValue(KeyAttr, x.Key);
            node.SetAttributeValue(DigestAttr, x.Digest);

            body.AppendChild(node);
        });

        return GetMemoryStream(doc);
    }

    public static MemoryStream MetaFieldsToHtml(IEnumerable<(string ResourceId, ContentEntity)> metafields)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();

        metafields.ToList().ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.InnerHtml = x.Item2.Value;
            node.SetAttributeValue(ResourceAttr, x.ResourceId);
            node.SetAttributeValue(KeyAttr, x.Item2.Key);
            node.SetAttributeValue(DigestAttr, x.Item2.Digest);

            body.AppendChild(node);
        });

        return GetMemoryStream(doc);
    }

    public static IEnumerable<TranslatableResourceContentRequest> ToJson(Stream file, string locale)
    {
        var contentNodes = GetContentNodes(file);

        return contentNodes.Select(x => new TranslatableResourceContentRequest()
        {
            Key = x.Attributes[KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[DigestAttr]?.Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale
        });
    }

    public static IEnumerable<TranslatableMetaFieldContentRequest> MetaFieldsToJson(Stream file, string locale)
    {
        var contentNodes = GetContentNodes(file);

        return contentNodes.Select(x => new TranslatableMetaFieldContentRequest()
        {
            ResourceId = x.Attributes[ResourceAttr].Value,
            Key = x.Attributes[KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[DigestAttr]?.Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale
        });
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument()
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement(HtmlConstants.Head));

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }

    private static MemoryStream GetMemoryStream(HtmlDocument doc)
    {
        var result = new MemoryStream();
        doc.Save(result);

        result.Position = 0;
        return result;
    }

    private static IEnumerable<HtmlNode> GetContentNodes(Stream file)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        return doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null);
    }
}