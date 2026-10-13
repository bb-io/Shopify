using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.ArticleImage;

public class SearchArticleImagesRequest
{
    [Display("Article image alt contains")]
    public string? NameContains { get; set; }
}