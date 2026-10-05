using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Filter;

public class UploadFilterRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Filter ID"), DataSource(typeof(FilterDataHandler))]
    public string? FilterId { get; set; }
}