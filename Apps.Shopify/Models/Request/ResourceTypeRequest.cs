using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request;

public class ResourceTypeRequest
{
    [StaticDataSource(typeof(ResourceTypeDataHandler))]
    public string ResourceType { get; set; }
}