using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.ArticleImage;

public class UploadArticleImageRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Article image ID"), DataSource(typeof(ArticleImageDataHandler))]
    public string? ArticleImageId { get; set; }
}