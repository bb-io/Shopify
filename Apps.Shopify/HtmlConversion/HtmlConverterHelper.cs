using System.Web;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.HtmlConversion.Models;
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
            ResourceId = x.Attributes[HtmlAttributeConstants.ResourceAttr]?.Value ?? string.Empty,
            Key = x.Attributes[HtmlAttributeConstants.KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[HtmlAttributeConstants.DigestAttr]?.Value ?? string.Empty,
            Value = GetValue(x),
            Locale = locale,
            MarketId = marketId
        }).ToList() ?? [];
    }
    
    public static void FillInIdentifiedContentEntities(
        HtmlDocument doc,
        HtmlNode body,
        IEnumerable<IdentifiedContentEntity> contentEntities)
    {
        contentEntities
            .Where(x => x.Value?.StartsWith("gid://shopify/") is false)
            .ToList()
            .ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            bool isRichText = RichTextConverter.TryToHtml(x.Value, out var richTextHtml);

            node.InnerHtml = LiquidPlaceholder.LockHtml(isRichText ? richTextHtml : x.Value ?? string.Empty);
            node.SetAttributeValue(HtmlAttributeConstants.KeyAttr, x.Key);
            node.SetAttributeValue(HtmlAttributeConstants.DigestAttr, x.Digest);
            node.SetAttributeValue(HtmlAttributeConstants.ResourceAttr, x.Id);

            if (isRichText)
                node.SetAttributeValue(HtmlAttributeConstants.FormatAttr, RichTextConverter.Format);
            
            body.AppendChild(node);
        });
    }
    
    private static string GetValue(HtmlNode node)
    {
        var html = LiquidPlaceholder.Unlock(node.InnerHtml);

        return node.Attributes[HtmlAttributeConstants.FormatAttr]?.Value == RichTextConverter.Format
            ? RichTextConverter.ToJson(html)
            : HttpUtility.HtmlDecode(html);
    }
}