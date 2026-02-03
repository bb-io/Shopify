using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class CollectionIdentifier
{
    [Display("Collection ID"), DataSource(typeof(CollectionDataHandler))]
    public string CollectionId { get; set; }
}