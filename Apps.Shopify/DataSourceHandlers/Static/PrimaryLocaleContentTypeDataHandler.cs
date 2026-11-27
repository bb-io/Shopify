using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PrimaryLocaleContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
        new[] { TranslatableResources.Product, TranslatableResources.Metafield,
            TranslatableResources.Metaobject, TranslatableResources.Theme }.Select(x => new DataSourceItem(x, x));
}
