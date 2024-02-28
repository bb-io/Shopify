using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Product;

public class ProductTranslationRequest
{
    [Display("Product ID")]
    [DataSource(typeof(ProductDataHandler))]
    public string ProductId { get; set; }

    [DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }
}