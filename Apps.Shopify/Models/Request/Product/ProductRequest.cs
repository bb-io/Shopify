using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Product;

public class ProductRequest
{
    [Display("Product ID")]
    [DataSource(typeof(ProductDataHandler))]
    public string ProductId { get; set; }
}