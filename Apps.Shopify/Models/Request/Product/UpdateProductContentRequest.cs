using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Product;

public class UpdateProductContentRequest
{
    [Display("Product ID")]
    [DataSource(typeof(ProductDataHandler))]
    public string ProductId { get; set; }

    [DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }

    public FileReference File { get; set; }
}