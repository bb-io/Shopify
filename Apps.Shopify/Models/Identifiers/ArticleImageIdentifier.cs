using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class ArticleImageIdentifier
{
    [Display("Article image ID"), DataSource(typeof(ArticleDataHandler))]
    public string ArticleImageId { get; set; } = string.Empty;
}