using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ProductStatusHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData() => new()
    {
        { "ACTIVE", "Active" },
        { "ARCHIVED", "Archived" },
        { "DRAFT", "Draft" },
    };
}