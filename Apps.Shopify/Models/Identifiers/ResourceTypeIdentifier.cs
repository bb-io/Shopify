using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Identifiers;

public class ResourceTypeIdentifier
{
    [Display("Resource type"), StaticDataSource(typeof(ResourceTypeDataHandler))]
    public string ResourceType { get; set; }
}