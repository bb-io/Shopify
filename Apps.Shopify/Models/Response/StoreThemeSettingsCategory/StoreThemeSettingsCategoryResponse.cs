using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeSettingsCategory;

public class StoreThemeSettingsCategoryResponse(ContentItemEntity contentEntity)
{
    [Display("Category ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Category name")]
    public string Name { get; set; } = contentEntity.Name;
}