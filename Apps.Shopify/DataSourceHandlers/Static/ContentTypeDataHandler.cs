using Apps.Shopify.Extensions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResource.COLLECTION.ToString(), TranslatableResource.COLLECTION.ToTitle()),
            new DataSourceItem(TranslatableResource.METAFIELD.ToString(), TranslatableResource.METAFIELD.ToTitle()),
            new DataSourceItem(TranslatableResource.ARTICLE.ToString(), TranslatableResource.ARTICLE.ToTitle()),
            new DataSourceItem(TranslatableResource.BLOG.ToString(), TranslatableResource.BLOG.ToTitle()),
            new DataSourceItem(TranslatableResource.PAGE.ToString(), TranslatableResource.PAGE.ToTitle()),
            new DataSourceItem(TranslatableResource.ONLINE_STORE_THEME.ToString(), TranslatableResource.ONLINE_STORE_THEME.ToTitle()),
            new DataSourceItem(TranslatableResource.PRODUCT.ToString(), TranslatableResource.PRODUCT.ToTitle()),
        ];
    }
}