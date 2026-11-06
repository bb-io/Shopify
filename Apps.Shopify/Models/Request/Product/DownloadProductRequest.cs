using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Product;

public class DownloadProductRequest
{
    [Display("Metafield keys", Description = "Download only these metafields")]
    [DataSource(typeof(ProductMetafieldKeyDataHandler))]
    public IEnumerable<string>? MetafieldKeys { get; set; }
    
    [Display("Include all metafields")]
    public bool? IncludeMetafields { get; set; }
    
    [Display("Include options")]
    public bool? IncludeOptions { get; set; }
    
    [Display("Include option values")]
    public bool? IncludeOptionValues { get; set; }
}