using System.Web;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.TranslatableResource;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion;

public static class HtmlConverterHelper
{
    public static IEnumerable<IdentifiedContentRequest> GetIdentifiedResourceContent(
        IEnumerable<HtmlNode>? nodes,
        string locale,
        string? marketId = null)
    {
        return nodes?.Select(x => new IdentifiedContentRequest
        {
            ResourceId = x.Attributes[HtmlAttributeConstants.ResourceAttr].Value,
            Key = x.Attributes[HtmlAttributeConstants.KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[HtmlAttributeConstants.DigestAttr].Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale,
            MarketId = marketId
        }) ?? [];
    }
    
    internal static void FillInIdentifiedContentEntities(
        HtmlDocument doc,
        HtmlNode body,
        IEnumerable<IdentifiedContentEntity> contentEntities)
    {
        contentEntities.ToList().ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.InnerHtml = x.Value;
            node.SetAttributeValue(HtmlAttributeConstants.KeyAttr, x.Key);
            node.SetAttributeValue(HtmlAttributeConstants.DigestAttr, x.Digest);
            node.SetAttributeValue(HtmlAttributeConstants.ResourceAttr, x.Id);

            body.AppendChild(node);
        });
    }
}