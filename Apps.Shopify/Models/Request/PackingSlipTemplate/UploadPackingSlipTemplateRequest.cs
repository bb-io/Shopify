using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.PackingSlipTemplate;

public class UploadPackingSlipTemplateRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Packing slip template ID"), DataSource(typeof(PackingSlipTemplateDataHandler))]
    public string? PackingSlipTemplateId { get; set; }
}