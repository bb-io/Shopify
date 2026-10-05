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

namespace Apps.Shopify.HtmlConversion.EmailTemplate;

public static class EmailTemplateHtmlConverter
{
    private const string TitleKey = "title";
    private const string BodyHtmlKey = "body_html";
    
    private static readonly Regex BlackbirdMetaRegex = new(
        @"<meta\s+name=""blackbird-[^""]*""[^>]*>", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex TitleRegex = new(
        $@"<div\s+{HtmlAttributeConstants.KeyAttr}=""{TitleKey}""[^>]*>.*?</div>", 
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex HeadTagRegex = new(@"<head\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex BodyTagRegex = new(@"<body\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Doesn't use HAP because it messes up the final HTML by closing tags in Shopify if statements
    public static MemoryStream ToHtml(IEnumerable<IdentifiedContentEntity> contentEntities, ShopifyMetadata metadata)
    {
        var contentEntitiesList = contentEntities.ToList();
        var body = contentEntitiesList.FirstOrDefault(x => x.Key == BodyHtmlKey) ?? 
                   throw new PluginMisconfigurationException($"Email template has no {BodyHtmlKey} content");

        if (string.IsNullOrWhiteSpace(body.Value))
            throw new PluginMisconfigurationException($"Email template {BodyHtmlKey} content is empty");
        
        string html = LiquidPlaceholder.LockHtml(body.Value);
        html = InsertAfter(html, BodyTagRegex, BuildTitle(contentEntitiesList.Where(x => x.Key == TitleKey)));
        html = InsertAfter(html, HeadTagRegex, BuildMetas(metadata, body.Digest));

        return new MemoryStream(Encoding.UTF8.GetBytes(html));
    }
    
    // Safe to parse using HAP because we don't save it again, therefore it shouldn't break the tags
    public static List<IdentifiedContentRequest> ToJson(string file, string locale, string? marketId)
    {
        var html = LiquidPlaceholder.Unlock(file);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var metadata = doc.GetAllMeta().Merge(new ShopifyMetadata { MarketId = marketId });
        var titleNodes = doc.DocumentNode.SelectNodes($"//div[@{HtmlAttributeConstants.KeyAttr}='{TitleKey}']");
        var titleResources = HtmlConverterHelper.GetIdentifiedResourceContent(titleNodes, locale, metadata.MarketId).ToList();

        string? bodyDigest = doc.GetMeta(HtmlMetadataConstants.BlackbirdBodyDigest);
        if (string.IsNullOrWhiteSpace(bodyDigest))
            throw new PluginMisconfigurationException("The file has no email body digest. Download the email template again");

        string? resourceId = titleResources.FirstOrDefault()?.ResourceId;
        if (string.IsNullOrWhiteSpace(resourceId))
            throw new PluginMisconfigurationException("The file has no title resource ID. Download the email template again");
        
        var body = new IdentifiedContentRequest
        {
            ResourceId = resourceId,
            Key = BodyHtmlKey,
            TranslatableContentDigest = bodyDigest,
            Value = RemoveMetadata(html),
            Locale = locale,
            MarketId = metadata.MarketId
        };

        return [..titleResources, body];
    }

    private static string RemoveMetadata(string html)
    {
        return TitleRegex.Replace(BlackbirdMetaRegex.Replace(html, string.Empty), string.Empty, 1);
    }

    private static string InsertAfter(string html, Regex tagRegex, string insertion)
    {
        var match = tagRegex.Match(html);
        return html.Insert(match.Success ? match.Index + match.Length : 0, insertion);
    }

    private static string BuildMetas(ShopifyMetadata metadata, string bodyDigest)
    {
        var doc = new HtmlDocument();
        var head = doc.CreateElement(HtmlConstants.Head);

        doc.AddMeta(head, HtmlMetadataConstants.BlackbirdContentType, metadata.ContentType)
            .AddMeta(head, HtmlMetadataConstants.BlackbirdMarketId, metadata.MarketId)
            .AddMeta(head, HtmlMetadataConstants.BlackbirdBodyDigest, bodyDigest);

        return head.InnerHtml;
    }

    private static string BuildTitle(IEnumerable<IdentifiedContentEntity> title)
    {
        var doc = new HtmlDocument();
        var body = doc.CreateElement(HtmlConstants.Body);

        HtmlConverterHelper.FillInIdentifiedContentEntities(doc, body, title);
        return body.InnerHtml;
    }
}