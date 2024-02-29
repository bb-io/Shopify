using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

public class ProductStatusHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "ACTIVE", "Active" },
        { "ARCHIVED", "Archived" },
        { "DRAFT", "Draft" },
    };
}