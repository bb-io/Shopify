using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.HtmlConversion.Constants;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Shopify.HtmlConversion;

public static class ShopifyHtmlConverter
{
    private const string KeyAttr = HtmlAttributeConstants.KeyAttr;
    private const string TypeAttr = HtmlAttributeConstants.TypeAttr;

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
        doc.LoadHtml(file);

        var contentType = doc.DocumentNode
            .SelectSingleNode($"//meta[@name='{HtmlMetadataConstants.BlackbirdContentType}']")?
            .GetAttributeValue("content", null);

        return contentType;
    }

    #endregion

    #region Blog

    public static MemoryStream BlogToHtml(
        IEnumerable<IdentifiedContentEntity> contentEntities,
        ICollection<IdentifiedContentEntity> blogPostsEntities, 
        ShopifyMetadata metadata)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(metadata);
        HtmlConverterHelper.FillInIdentifiedContentEntities(doc, body, contentEntities);

        if (blogPostsEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, BlogPostType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, blogPostsEntities);
        }

        return GetMemoryStream(doc);
    }

    public static (IEnumerable<IdentifiedContentRequest> blog, IEnumerable<IdentifiedContentRequest> blogPosts) BlogToJson(
        string file,
        string locale,
        ShopifyMetadata overrides)
    {
        var (doc, metadata) = LoadDocument(file, overrides);
        
        var blogContentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null && x.ParentNode.Name == "body");

        var blogPostsContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == BlogPostType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);

        var blog = HtmlConverterHelper.GetIdentifiedResourceContent(blogContentNodes, locale, metadata.MarketId);
        var blogPosts = HtmlConverterHelper.GetIdentifiedResourceContent(blogPostsContentNodes, locale, metadata.MarketId);

        return (blog, blogPosts);
    }

    #endregion

    #region Product

    public static MemoryStream ProductToHtml(ProductContentDto contentDto)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(new ShopifyMetadata
        {
            ContentType = TranslatableResources.Product.ToLower(),
            MarketId = contentDto.MarketId
        });
        HtmlConverterHelper.FillInIdentifiedContentEntities(doc, body, contentDto.ProductContentEntities);

        if (contentDto.MetafieldsContentEntities is not null && contentDto.MetafieldsContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, MetafieldType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.MetafieldsContentEntities);
        }      
        
        if (contentDto.OptionsContentEntities is not null && contentDto.OptionsContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, OptionType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.OptionsContentEntities);
        }      
        
        if (contentDto.OptionValuesContentEntities is not null && contentDto.OptionValuesContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, OptionValueType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.OptionValuesContentEntities);
        }

        return GetMemoryStream(doc);
    }

    public static ProductTranslatableResourceDto ProductToJson(string file, string locale, string? marketId = null)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(file);
        
        var productContentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[KeyAttr]?.Value != null && x.ParentNode.Name == "body");

        if (!productContentNodes.Any())
            throw new PluginMisconfigurationException("Invalid product HTML: no product nodes with 'key' attribute found.");

        var metafieldContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == MetafieldType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var optionContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == OptionType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);
        
        var optionValuesContentNodes = doc.DocumentNode.Descendants()
            .FirstOrDefault(x => x.Attributes[TypeAttr]?.Value == OptionValueType)?
            .ChildNodes.Where(x => x.Attributes[KeyAttr]?.Value != null);

        marketId ??= doc.GetMeta(HtmlMetadataConstants.BlackbirdMarketId);
        var product = HtmlConverterHelper.GetIdentifiedResourceContent(productContentNodes, locale, marketId);
        var metafields = HtmlConverterHelper.GetIdentifiedResourceContent(metafieldContentNodes, locale, marketId);
        var options = HtmlConverterHelper.GetIdentifiedResourceContent(optionContentNodes, locale, marketId);
        var optionValues = HtmlConverterHelper.GetIdentifiedResourceContent(optionValuesContentNodes, locale, marketId);

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
        var metadata = new ShopifyMetadata { ContentType = TranslatableResources.Store.ToLower() };
        var (doc, body) = PrepareEmptyHtmlDocument(metadata);

        if (contentDto.ThemesContentEntities is not null && contentDto.ThemesContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ThemeType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.ThemesContentEntities);
        }      
        
        if (contentDto.MenuContentEntities is not null && contentDto.MenuContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, MenuType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.MenuContentEntities);
        }      
        
        if (contentDto.ShopContentEntities is not null && contentDto.ShopContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ShopType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.ShopContentEntities);
        }      
        
        if (contentDto.ShopPolicyContentEntities is not null && contentDto.ShopPolicyContentEntities.Any())
        {
            var node = doc.CreateElement(HtmlConstants.Div);
            node.SetAttributeValue(TypeAttr, ShopPolicyType);
            body.AppendChild(node);

            HtmlConverterHelper.FillInIdentifiedContentEntities(doc, node, contentDto.ShopPolicyContentEntities);
        }

        return GetMemoryStream(doc);
    }

    public static ShopTranslatableResourceDto StoreToJson(string file, string locale)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(file);

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

        var themes = HtmlConverterHelper.GetIdentifiedResourceContent(themeContentNodes, locale);
        var menu = HtmlConverterHelper.GetIdentifiedResourceContent(menuContentNodes, locale);
        var shop = HtmlConverterHelper.GetIdentifiedResourceContent(shopContentNodes, locale);
        var shopPolicy = HtmlConverterHelper.GetIdentifiedResourceContent(shopPolicyContentNodes, locale);

        return new()
        {
            ThemesContentEntities = themes,
            MenuContentEntities = menu,
            ShopContentEntities = shop,
            ShopPolicyContentEntities = shopPolicy
        };
    }

    #endregion
    
    public static MemoryStream ToHtml(IEnumerable<IdentifiedContentEntity> contentEntities, ShopifyMetadata metadata)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(metadata);
        HtmlConverterHelper.FillInIdentifiedContentEntities(doc, body, contentEntities);

        return GetMemoryStream(doc);
    }
    
    public static IEnumerable<IdentifiedContentRequest> ToJson(string file, string locale, ShopifyMetadata overrides)
    {
        var (doc, metadata) = LoadDocument(file, overrides);

        var mergedMetadata = doc.GetAllMeta().Merge(metadata);
        var contentNodes = doc.DocumentNode.Descendants().Where(x => x.Attributes[KeyAttr]?.Value != null);

        return HtmlConverterHelper.GetIdentifiedResourceContent(contentNodes, locale, mergedMetadata.MarketId);
    }
    
    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(ShopifyMetadata metadata)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement(HtmlConstants.Head);
        htmlNode.AppendChild(headNode);

        htmlDoc.AddMeta(headNode, HtmlMetadataConstants.BlackbirdContentType, metadata.ContentType);
        htmlDoc.AddMeta(headNode, HtmlMetadataConstants.BlackbirdMarketId, metadata.MarketId);
        
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
    
    private static (HtmlDocument doc, ShopifyMetadata metadata) LoadDocument(string file, ShopifyMetadata? overrides = null)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(file);

        var mergedMeta = doc.GetAllMeta().Merge(overrides);
        return (doc, mergedMeta);
    }
}