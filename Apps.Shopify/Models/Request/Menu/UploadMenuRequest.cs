using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Menu;

public class UploadMenuRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Menu ID"), DataSource(typeof(MenuDataHandler))]
    public string? MenuId { get; set; }
}