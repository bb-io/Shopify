using Apps.Shopify.Models.Entities.Menu;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Menu;

public class MenuResponse(MenuEntity entity)
{
    [Display("Menu ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Menu title")]
    public string Title { get; set; } = entity.Title;

    [Display("Menu handle")]
    public string Handle { get; set; } = entity.Handle;
}