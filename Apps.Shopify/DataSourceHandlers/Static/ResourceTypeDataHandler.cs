using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ResourceTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            ["LINK"] = "Link",
            ["METAOBJECT"] = "Metaobject"
        };
    }
}