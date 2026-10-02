using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StoreThemeLocaleContent;

public class UploadStoreThemeLocaleContentRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Store theme locale content ID"), DataSource(typeof(ThemeLocaleContentDataHandler))]
    public string? ThemeLocaleContentId { get; set; }
}