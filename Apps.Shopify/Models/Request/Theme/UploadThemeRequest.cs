using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStoreTheme;

public class UploadThemeRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Theme ID"), DataSource(typeof(ThemeDataHandler))]
    public string? ThemeId { get; set; }
}
