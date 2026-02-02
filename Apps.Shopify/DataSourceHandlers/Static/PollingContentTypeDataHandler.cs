using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PollingContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResources.Article, TranslatableResources.Article),
            new DataSourceItem(TranslatableResources.Blog, TranslatableResources.Blog),
            new DataSourceItem(TranslatableResources.Collection, TranslatableResources.Collection),
            new DataSourceItem(TranslatableResources.Page, TranslatableResources.Page),
            new DataSourceItem(TranslatableResources.Product, TranslatableResources.Product),
        ];
    }
}
