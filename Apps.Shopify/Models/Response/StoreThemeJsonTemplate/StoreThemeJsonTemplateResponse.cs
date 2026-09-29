using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeJsonTemplate;

public class StoreThemeJsonTemplateResponse(ContentItemEntity contentEntity)
{
    [Display("Template ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Template name")]
    public string Name { get; set; } = contentEntity.Name;
}