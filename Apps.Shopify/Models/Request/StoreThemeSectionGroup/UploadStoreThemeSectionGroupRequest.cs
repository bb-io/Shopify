using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StoreThemeSectionGroup;

public class UploadStoreThemeSectionGroupRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Store theme section group ID"), DataSource(typeof(ThemeSectionGroupDataHandler))]
    public string? ThemeSectionGroupId { get; set; }
}