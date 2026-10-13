using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.CollectionImage;

public class SearchCollectionImagesRequest
{
    [Display("Collection image alt contains")]
    public string? NameContains { get; set; }
}