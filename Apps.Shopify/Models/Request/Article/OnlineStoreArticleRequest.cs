using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.OnlineStoreArticle;

public class OnlineStoreArticleRequest
{
    [Display("Online store article ID")]
    [DataSource(typeof(OnlineStoreArticleHandler))]
    public string OnlineStoreArticleId { get; set; }
}