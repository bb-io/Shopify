using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.MediaImage;

public class SearchMediaImagesRequest
{
    [Display("Media image name contains")]
    public string? NameContains { get; set; }
}