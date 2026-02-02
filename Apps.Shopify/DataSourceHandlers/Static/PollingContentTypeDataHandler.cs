using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PollingContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResource.COLLECTION.ToString(), TranslatableResource.COLLECTION.ToString().ToLower()),
            new DataSourceItem(TranslatableResource.ARTICLE.ToString(), TranslatableResource.ARTICLE.ToString().ToLower()),
            new DataSourceItem(TranslatableResource.BLOG.ToString(), TranslatableResource.BLOG.ToString().ToLower()),
            new DataSourceItem(TranslatableResource.ONLINE_STORE_THEME.ToString(), TranslatableResource.ONLINE_STORE_THEME.ToString().ToLower()),
            new DataSourceItem(TranslatableResource.PRODUCT.ToString(), TranslatableResource.PRODUCT.ToString().ToLower()),
        ];
    }
}
