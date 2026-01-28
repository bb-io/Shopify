using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.DataSourceHandlers.Static;

public class ThemeRoleDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return [
            new DataSourceItem("ARCHIVED", "Archived"),
            new DataSourceItem("DEMO", "Demo"),
            new DataSourceItem("DEVELOPMENT", "Development"),
            new DataSourceItem("LOCKED", "Locked"),
            new DataSourceItem("MAIN", "Main"),
            new DataSourceItem("UNPUBLISHED", "Unpublished"),
        ];
    }
}
