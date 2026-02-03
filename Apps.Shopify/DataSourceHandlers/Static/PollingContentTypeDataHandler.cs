using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class PollingContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return TranslatableResources.SupportedPollingContentTypes.Select(x => new DataSourceItem(x, x));
    }
}
