using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Models.Request.Product;

public class SearchProductsRequest
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Status"), StaticDataSource(typeof(ProductStatusHandler))]
    public string? Status { get; set; }
    
    [Display("Metafield key"), DataSource(typeof(ProductMetafieldKeyDataHandler))]
    public string? MetafieldKey { get; set; }
    
    [Display("Metafield value contains")]
    public string? MetafieldValueContains { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(MetafieldKey) ^ string.IsNullOrEmpty(MetafieldValueContains))
            throw new PluginMisconfigurationException("Metafield and Metafield value should be both either filled or empty");
    }
}