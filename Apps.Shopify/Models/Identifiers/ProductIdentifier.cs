using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class ProductIdentifier
{
    [Display("Product ID"), DataSource(typeof(ProductDataHandler))]
    public string ProductId { get; set; }
}