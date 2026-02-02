using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem(TranslatableResources.Collection, TranslatableResources.Collection),
            new DataSourceItem(TranslatableResources.Metafield, TranslatableResources.Metafield),
            new DataSourceItem(TranslatableResources.Article, TranslatableResources.Article),
            new DataSourceItem(TranslatableResources.Blog, TranslatableResources.Blog),
            new DataSourceItem(TranslatableResources.Page, TranslatableResources.Page),
            new DataSourceItem(TranslatableResources.Theme, TranslatableResources.Theme),
            new DataSourceItem(TranslatableResources.Product, TranslatableResources.Product),
        ];
    }
}