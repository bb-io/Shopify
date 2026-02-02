using Apps.Shopify.Extensions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PollingContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResource.COLLECTION.ToString(), TranslatableResource.COLLECTION.ToTitle()),
            new DataSourceItem(TranslatableResource.ARTICLE.ToString(), TranslatableResource.ARTICLE.ToTitle()),
            new DataSourceItem(TranslatableResource.BLOG.ToString(), TranslatableResource.BLOG.ToTitle()),
            new DataSourceItem(TranslatableResource.ONLINE_STORE_THEME.ToString(), TranslatableResource.ONLINE_STORE_THEME.ToTitle()),
            new DataSourceItem(TranslatableResource.PRODUCT.ToString(), TranslatableResource.PRODUCT.ToTitle()),
        ];
    }
}
