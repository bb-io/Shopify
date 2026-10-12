using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StoreThemeSettingsDataSection;

public class UploadStoreThemeSettingsDataSectionRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Store theme settings data section ID"), DataSource(typeof(ThemeSettingsDataSectionDataHandler))]
    public string? ThemeSettingsDataSectionId { get; set; }
}