using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StoreThemeAppEmbed;

public class UploadStoreThemeAppEmbedRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Store theme JSON template ID"), DataSource(typeof(ThemeAppEmbedDataHandler))]
    public string? ThemeAppEmbedIdId { get; set; }
}