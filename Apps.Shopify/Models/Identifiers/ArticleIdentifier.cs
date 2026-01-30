using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class ArticleIdentifier
{
    [Display("Article ID")]
    [DataSource(typeof(OnlineStoreArticleHandler))]
    public string ArticleId { get; set; }
}