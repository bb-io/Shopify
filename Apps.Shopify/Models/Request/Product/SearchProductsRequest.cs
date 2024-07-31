using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Product;

public class SearchProductsRequest
{
    [StaticDataSource(typeof(ProductStatusHandler))]
    public string? Status { get; set; }
    
    [Display("Metafield")]
    [DataSource(typeof(ProductMetafieldDataHandler))]
    public string? MetafieldKey { get; set; }
    
    [Display("Metafield value")]
    public string? MetafieldValue { get; set; }
}