using System.Web;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.Product;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion;

public static class ShopifyHtmlConverter
{
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

        var result = new MemoryStream();
        doc.Save(result);

        result.Position = 0;
        return result;
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

    public static IEnumerable<ProductContentRequest> ToJson(Stream file, string locale)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var contentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null)
            .ToList();
        
        return contentNodes.Select(x => new ProductContentRequest()
        {
            Key = x.Attributes[KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[DigestAttr]?.Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale
        });
    }
}