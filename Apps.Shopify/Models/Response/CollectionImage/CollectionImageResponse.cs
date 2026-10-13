using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.CollectionImage;

public class CollectionImageResponse(ContentItemEntity contentEntity)
{
    [Display("Collection image ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Collection image alt")]
    public string Name { get; set; } = contentEntity.Name;
}