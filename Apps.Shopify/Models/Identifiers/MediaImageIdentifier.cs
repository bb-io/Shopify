using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class MediaImageIdentifier
{
    [Display("Media image ID"), DataSource(typeof(MediaImageDataHandler))]
    public string MediaImageId { get; set; } = string.Empty;
}