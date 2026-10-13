using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.StorePolicy;

public class UploadStorePolicyRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Store policy ID"), DataSource(typeof(StorePolicyDataHandler))]
    public string? StorePolicyId { get; set; }
}