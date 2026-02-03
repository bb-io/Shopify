using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Product;

public class DownloadProductRequest
{
    [Display("Include metafields")]
    public bool? IncludeMetafields { get; set; }
    
    [Display("Include options")]
    public bool? IncludeOptions { get; set; }
    
    [Display("Include option values")]
    public bool? IncludeOptionValues { get; set; }
}