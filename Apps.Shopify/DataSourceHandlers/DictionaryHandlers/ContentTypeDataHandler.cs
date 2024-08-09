using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ContentTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>
        {
            { HtmlContentTypes.OnlineStoreArticle, "Online store article" },
            { HtmlContentTypes.Collection, "Collection" },
            { HtmlContentTypes.MetafieldContent, "Metafield" },
            { HtmlContentTypes.OnlineStoreBlogContent, "Store blog" },
            { HtmlContentTypes.OnlineStorePageContent, "Store page" },
            { HtmlContentTypes.OnlineStoreThemeContent, "Store theme" },
            { HtmlContentTypes.ProductContent, "Product" },
            { HtmlContentTypes.StoreResourcesContent, "Store resources" },
            { HtmlContentTypes.StoreContent, "Store" }
        };
    }
}