using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Theme;

public class DownloadThemeRequest
{
    [Display("Asset keys", Description = "Specific assets to translate"), DataSource(typeof(AssetThemeDataHandler))]
    public IEnumerable<string>? AssetKeys { get; set; }
}