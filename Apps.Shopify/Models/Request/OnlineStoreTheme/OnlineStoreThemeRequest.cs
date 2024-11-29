using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.OnlineStoreTheme;

public class OnlineStoreThemeRequest
{
    [Display("Online store theme ID")]
    [DataSource(typeof(OnlineStoreThemeDataSourceHandler))]
    public string OnlineStoreThemeId { get; set; } = default!;
}