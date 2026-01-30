using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.OnlineStorePage;

public class OnlineStorePageRequest
{
    [Display("Online store page ID")]
    [DataSource(typeof(PageDataHandler))]
    public string OnlineStorePageId { get; set; }
}