using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.MediaImage;

public class MediaImageResponse(ContentItemEntity contentEntity)
{
    [Display("Media image ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Media image name")]
    public string Name { get; set; } = contentEntity.Name;
}