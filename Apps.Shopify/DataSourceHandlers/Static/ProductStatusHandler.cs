using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class ProductStatusHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return [
            new DataSourceItem("ACTIVE", "Active"),
            new DataSourceItem("ARCHIVED", "Archived"),
            new DataSourceItem("DRAFT", "Draft")
        ];
    }
}