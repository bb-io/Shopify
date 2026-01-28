using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Metafield;

public class SearchMetafieldsRequest
{
    [Display("Owner type"), StaticDataSource(typeof(MetafieldOwnerTypeDataHandler))]
    public string OwnerType { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }

    [Display("Key")]
    public string? Key { get; set; }

    [Display("Namespace")]
    public string? Namespace { get; set; }
}
