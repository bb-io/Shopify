using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeAppEmbed;

public class StoreThemeAppEmbedResponse(ContentItemEntity contentEntity)
{
    [Display("App embed ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("App embed name")]
    public string Name { get; set; } = contentEntity.Name;
}