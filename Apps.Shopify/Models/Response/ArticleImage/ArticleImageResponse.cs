using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.ArticleImage;

public class ArticleImageResponse(ContentItemEntity contentEntity)
{
    [Display("Article image ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Article image alt")]
    public string Name { get; set; } = contentEntity.Name;
}