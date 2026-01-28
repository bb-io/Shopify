using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStoreArticle;

public class UploadArticleRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Article ID"), DataSource(typeof(OnlineStoreArticleHandler))]
    public string? ArticleId { get; set; }
}
