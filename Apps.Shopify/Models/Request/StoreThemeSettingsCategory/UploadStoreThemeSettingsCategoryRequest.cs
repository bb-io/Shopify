using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StoreThemeSettingsCategory;

public class UploadStoreThemeSettingsCategoryRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Store theme settings category ID"), DataSource(typeof(ThemeSettingsCategoryDataHandler))]
    public string? ThemeSettingsCategoryId { get; set; }
}