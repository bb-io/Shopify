using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.PackingSlipTemplate;

public class SearchPackingSlipTemplatesRequest
{
    [Display("Packing slip template name contains")]
    public string? NameContains { get; set; }
}