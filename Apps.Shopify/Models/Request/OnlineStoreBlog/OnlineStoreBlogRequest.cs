using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request.OnlineStoreBlog;

public class OnlineStoreBlogRequest
{
    [Display("Online store blog ID")]
    [DataSource(typeof(OnlineStoreBlogHandler))]
    public string OnlineStoreBlogId { get; set; }
}