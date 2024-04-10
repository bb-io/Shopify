using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Product;

public class SearchProductsRequest
{
    [StaticDataSource(typeof(ProductStatusHandler))]
    public string? Status { get; set; }
}