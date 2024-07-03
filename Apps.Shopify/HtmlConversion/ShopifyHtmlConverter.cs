using System.Web;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.TranslatableResource;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion;

public static class ShopifyHtmlConverter
{
    private const string ResourceAttr = "resource";
    private const string KeyAttr = "key";
    private const string TypeAttr = "type";
    private const string DigestAttr = "digest";

    private const string BlogPostType = "blogPost";
    private const string MetafieldType = "metafield";
    private const string OptionType = "option";
    private const string OptionValueType = "optionValue";

    #region Metafield

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

    public static IEnumerable<IdentifiedContentRequest> MetaFieldsToJson(Stream file, string locale)
    {
        var contentNodes = GetContentNodes(file);
        return GetIdentifiedResourceContent(contentNodes, locale);
    }

    #endregion

    #region Blog

    public static MemoryStream BlogToHtml(IEnumerable<IdentifiedContentEntity> contentEntities,
        ICollection<IdentifiedContentEntity> blogPostsEntities)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();
        FillInIdentifiedContentEntities(doc, body, contentEntities);

        if (blogPostsEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, BlogPostType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, blogPostsEntities);
        }

        return GetMemoryStream(doc);
    }

    public static (IEnumerable<IdentifiedContentRequest> blog,
        IEnumerable<IdentifiedContentRequest> blogPosts) BlogToJson(Stream file, string locale)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var blogContentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null && x.ParentNode.Name == "body");

        var blogPostsContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == BlogPostType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);

        var blog = GetIdentifiedResourceContent(blogContentNodes, locale);
        var blogPosts = GetIdentifiedResourceContent(blogPostsContentNodes, locale);

        return (blog, blogPosts);
    }

    #endregion

    #region Product

    public static MemoryStream ProductToHtml(ProductContentDto contentDto)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();
        FillInContentEntities(doc, body, contentDto.ProductContentEntities);

        if (contentDto.MetafieldsContentEntities is not null && contentDto.MetafieldsContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, MetafieldType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.MetafieldsContentEntities);
        }      
        
        if (contentDto.OptionsContentEntities is not null && contentDto.OptionsContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, OptionType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.OptionsContentEntities);
        }      
        
        if (contentDto.OptionValuesContentEntities is not null && contentDto.OptionValuesContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, OptionValueType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.OptionValuesContentEntities);
        }

        return GetMemoryStream(doc);
    }

    public static ProductTranslatableResourceDto ProductToJson(Stream file, string locale)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var productContentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null && x.ParentNode.Name == "body");

        var metafieldContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == MetafieldType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var optionContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == OptionType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var optionValuesContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == OptionValueType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);

        var product = GetIdentifiedResourceContent(productContentNodes, locale);
        var metafields = GetIdentifiedResourceContent(metafieldContentNodes, locale);
        var options = GetIdentifiedResourceContent(optionContentNodes, locale);
        var optionValues = GetIdentifiedResourceContent(optionValuesContentNodes, locale);

        return new()
        {
            ProductContentEntities = product,
            MetafieldsContentEntities = metafields,
            OptionsContentEntities = options,
            OptionValuesContentEntities = optionValues
        };
    }

    #endregion

    public static MemoryStream ToHtml(IEnumerable<ContentEntity> contentEntities)
    {
        var (doc, body) = PrepareEmptyHtmlDocument();
        FillInContentEntities(doc, body, contentEntities);

        return GetMemoryStream(doc);
    }

    public static IEnumerable<TranslatableResourceContentRequest> ToJson(Stream file, string locale)
    {
        var contentNodes = GetContentNodes(file);
        return GetResourceContent(contentNodes, locale);
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

    private static void FillInContentEntities(HtmlDocument doc, HtmlNode body,
        IEnumerable<ContentEntity> contentEntities)
    {
        contentEntities.ToList().ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.InnerHtml = x.Value;
            node.SetAttributeValue(KeyAttr, x.Key);
            node.SetAttributeValue(DigestAttr, x.Digest);

            body.AppendChild(node);
        });
    }   
    
    private static void FillInIdentifiedContentEntities(HtmlDocument doc, HtmlNode body,
        IEnumerable<IdentifiedContentEntity> contentEntities)
    {
        contentEntities.ToList().ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.InnerHtml = x.Value;
            node.SetAttributeValue(KeyAttr, x.Key);
            node.SetAttributeValue(DigestAttr, x.Digest);
            node.SetAttributeValue(ResourceAttr, x.Id);

            body.AppendChild(node);
        });
    }

    private static IEnumerable<TranslatableResourceContentRequest> GetResourceContent(IEnumerable<HtmlNode>? nodes,
        string locale) =>
        nodes?.Select(x => new TranslatableResourceContentRequest()
        {
            Key = x.Attributes[KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[DigestAttr]?.Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale
        }) ?? [];

    private static IEnumerable<IdentifiedContentRequest> GetIdentifiedResourceContent(IEnumerable<HtmlNode>? nodes,
        string locale) =>
        nodes?.Select(x => new IdentifiedContentRequest()
        {
            ResourceId = x.Attributes[ResourceAttr]?.Value,
            Key = x.Attributes[KeyAttr].Value,
            TranslatableContentDigest = x.Attributes[DigestAttr]?.Value,
            Value = HttpUtility.HtmlDecode(x.InnerHtml),
            Locale = locale
        }) ?? [];
}