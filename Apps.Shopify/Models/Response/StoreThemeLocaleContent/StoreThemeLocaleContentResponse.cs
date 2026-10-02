using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeLocaleContent;

public class StoreThemeLocaleContentResponse(ContentItemEntity contentEntity)
{
    [Display("Theme locale content ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Theme locale content name")]
    public string Name { get; set; } = contentEntity.Name;
}