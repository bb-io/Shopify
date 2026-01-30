using Apps.Shopify.Extensions;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PollingContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResource.COLLECTION.ToString(), TranslatableResource.COLLECTION.ToString().ToTitle()),
            new DataSourceItem(TranslatableResource.ARTICLE.ToString(), TranslatableResource.ARTICLE.ToString().ToTitle()),
            new DataSourceItem(TranslatableResource.BLOG.ToString(), TranslatableResource.BLOG.ToString().ToTitle()),
            new DataSourceItem(TranslatableResource.ONLINE_STORE_THEME.ToString(), TranslatableResource.ONLINE_STORE_THEME.ToString().ToTitle()),
            new DataSourceItem(TranslatableResource.PRODUCT.ToString(), TranslatableResource.PRODUCT.ToString().ToTitle()),
        ];
    }
}
