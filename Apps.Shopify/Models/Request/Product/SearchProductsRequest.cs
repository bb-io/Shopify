using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Product;

public class SearchProductsRequest
{
    [DataSource(typeof(ProductStatusHandler))]
    public string? Status { get; set; }
}