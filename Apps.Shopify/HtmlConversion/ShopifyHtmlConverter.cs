using System.Web;
using Apps.Shopify.Constants;
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
    private const string ThemeType = "theme";
    private const string MenuType = "storeMenu";
    private const string ShopType = "shop";
    private const string ShopPolicyType = "shopPolicy";

    #region Generic

    public static string? ExtractContentTypeFromHtml(string file)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var contentType = doc.DocumentNode
            .SelectSingleNode("//meta[@name='blackbird-content-type']")
            ?.GetAttributeValue("content", null);

        return contentType;
    }

    #endregion

    #region Metafield

    public static MemoryStream MetaFieldsToHtml(IEnumerable<(string ResourceId, ContentEntity)> metafields, string contentType)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(contentType);

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

    public static IEnumerable<IdentifiedContentRequest> MetaFieldsToJson(string file,
        string locale)
    {
        var contentNodes = GetContentNodes(file);
        return GetIdentifiedResourceContent(contentNodes, locale);
    }

    #endregion

    #region Blog

    public static MemoryStream BlogToHtml(IEnumerable<IdentifiedContentEntity> contentEntities,
        ICollection<IdentifiedContentEntity> blogPostsEntities, string contentType)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(contentType);
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
        IEnumerable<IdentifiedContentRequest> blogPosts) BlogToJson(string file, string locale)
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
        var (doc, body) = PrepareEmptyHtmlDocument(HtmlContentTypes.ProductContent);
        FillInIdentifiedContentEntities(doc, body, contentDto.ProductContentEntities);

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

    public static ProductTranslatableResourceDto ProductToJson(string file, string locale)
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

    #region Store

    public static MemoryStream StoreToHtml(StoreContentDto contentDto)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(HtmlContentTypes.StoreContent);

        if (contentDto.ThemesContentEntities is not null && contentDto.ThemesContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ThemeType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.ThemesContentEntities);
        }      
        
        if (contentDto.MenuContentEntities is not null && contentDto.MenuContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, MenuType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.MenuContentEntities);
        }      
        
        if (contentDto.ShopContentEntities is not null && contentDto.ShopContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ShopType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.ShopContentEntities);
        }      
        
        if (contentDto.ShopPolicyContentEntities is not null && contentDto.ShopPolicyContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ShopPolicyType);
            body.AppendChild(node);

            FillInIdentifiedContentEntities(doc, node, contentDto.ShopPolicyContentEntities);
        }

        return GetMemoryStream(doc);
    }

    public static ShopTranslatableResourceDto StoreToJson(string file, string locale)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var themeContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == ThemeType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var menuContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == MenuType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var shopContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == ShopType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var shopPolicyContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == ShopPolicyType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);

        var themes = GetIdentifiedResourceContent(themeContentNodes, locale);
        var menu = GetIdentifiedResourceContent(menuContentNodes, locale);
        var shop = GetIdentifiedResourceContent(shopContentNodes, locale);
        var shopPolicy = GetIdentifiedResourceContent(shopPolicyContentNodes, locale);

        return new()
        {
            ThemesContentEntities = themes,
            MenuContentEntities = menu,
            ShopContentEntities = shop,
            ShopPolicyContentEntities = shopPolicy
        };
    }

    #endregion
    
    public static MemoryStream ToHtml(IEnumerable<IdentifiedContentEntity> contentEntities, string contentType)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(contentType);
        FillInIdentifiedContentEntities(doc, body, contentEntities);

        return GetMemoryStream(doc);
    }
    
    public static IEnumerable<IdentifiedContentRequest> ToJson(string file, string locale)
    {
        var contentNodes = GetContentNodes(file);
        return GetIdentifiedResourceContent(contentNodes, locale);
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(string contentType)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement(HtmlConstants.Head);
        htmlNode.AppendChild(headNode);

        var contentTypeMetaNode = htmlDoc.CreateElement("meta");
        contentTypeMetaNode.SetAttributeValue("name", "blackbird-content-type");
        contentTypeMetaNode.SetAttributeValue("content", contentType);
        headNode.AppendChild(contentTypeMetaNode);

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

    private static IEnumerable<HtmlNode> GetContentNodes(string file)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        return doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null);
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