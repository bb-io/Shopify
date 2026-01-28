using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStorePage;

public class UploadPageRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Page ID"), DataSource(typeof(OnlineStorePageHandler))]
    public string? PageId { get; set; }
}
