using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class CollectionImageIdentifier
{
    [Display("Collection image ID"), DataSource(typeof(CollectionImageDataHandler))]
    public string CollectionImageId { get; set; } = string.Empty;
}