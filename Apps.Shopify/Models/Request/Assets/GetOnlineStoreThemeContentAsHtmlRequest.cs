using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.Models.Request.OnlineStoreTheme;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.Assets;

public class GetOnlineStoreThemeContentAsHtmlRequest : OnlineStoreThemeRequest
{
    [Display("Asset keys", Description = "Specify this input if you want to translate only specific assets"), DataSource(typeof(AssetThemeDataHandler))]
    public IEnumerable<string>? AssetKeys { get; set; }
}