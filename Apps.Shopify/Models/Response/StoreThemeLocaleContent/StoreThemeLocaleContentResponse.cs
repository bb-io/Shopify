using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeLocaleContent;

public class StoreThemeLocaleContentResponse(ContentItemEntity contentEntity)
{
    [Display("Content ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Content name")]
    public string Name { get; set; } = contentEntity.Name;
}