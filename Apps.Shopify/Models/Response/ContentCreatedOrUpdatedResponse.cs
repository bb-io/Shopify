using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response;

public class ContentCreatedOrUpdatedResponse
{
    [Display("Article IDs")]
    public List<string> ArticleIds { get; set; } = new();

    [Display("Page IDs")]
    public List<string> PageIds { get; set; } = new();
}