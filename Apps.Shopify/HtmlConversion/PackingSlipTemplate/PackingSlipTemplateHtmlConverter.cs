using System.Text;
using System.Text.RegularExpressions;
using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion.PackingSlipTemplate;

public static class PackingSlipTemplateHtmlConverter
{
    private const string BodyKey = "body";
    private static readonly Regex BodyContentRegex = new(
        @"<body\b[^>]*>(.*)</body>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    // Doesn't use HAP so it won't break Liquid
    public static MemoryStream ToHtml(IEnumerable<IdentifiedContentEntity> contentEntities, ShopifyMetadata metadata)
    {
        var body = contentEntities.FirstOrDefault(x => x.Key == BodyKey) ??
                   throw new PluginMisconfigurationException($"Packing slip template has no {BodyKey} content");

        if (string.IsNullOrWhiteSpace(body.Value))
            throw new PluginMisconfigurationException($"Packing slip template {BodyKey} content is empty");

        string meta = BuildMetas(metadata, body);
        string html = $"<html><head>{meta}</head><body>{LiquidPlaceholder.LockHtml(body.Value)}</body></html>";
        return new MemoryStream(Encoding.UTF8.GetBytes(html));
    }

    // Safe to parse using HAP because we only read metas from it
    public static List<IdentifiedContentRequest> ToJson(string file, string locale, string? marketId)
    {
        string html = LiquidPlaceholder.Unlock(file);
        
        var bodyMatch = BodyContentRegex.Match(html);
        string bodyContent = bodyMatch.Success
            ? bodyMatch.Groups[1].Value
            : throw new PluginMisconfigurationException($"The file has no {BodyKey}. Download the packing slip template again");

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var metadata = doc.GetAllMeta().Merge(new ShopifyMetadata { MarketId = marketId });

        return
        [
            new IdentifiedContentRequest
            {
                ResourceId = doc.GetRequiredMeta(HtmlMetadataConstants.BlackbirdResourceId),
                Key = BodyKey,
                TranslatableContentDigest = doc.GetRequiredMeta(HtmlMetadataConstants.BlackbirdBodyDigest),
                Value = bodyContent,
                Locale = locale,
                MarketId = metadata.MarketId
            }
        ];
    }

    private static string BuildMetas(ShopifyMetadata metadata, IdentifiedContentEntity body)
    {
        var doc = new HtmlDocument();
        var head = doc.CreateElement(HtmlConstants.Head);

        doc.AddMeta(head, HtmlMetadataConstants.BlackbirdContentType, metadata.ContentType)
            .AddMeta(head, HtmlMetadataConstants.BlackbirdMarketId, metadata.MarketId)
            .AddMeta(head, HtmlMetadataConstants.BlackbirdResourceId, body.Id)
            .AddMeta(head, HtmlMetadataConstants.BlackbirdBodyDigest, body.Digest);

        return head.InnerHtml;
    }
}