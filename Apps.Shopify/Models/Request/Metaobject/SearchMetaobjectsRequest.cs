using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Metaobject;

public class SearchMetaobjectsRequest
{
    [Display("Metaobject name contains")]
    public string? NameContains { get; set; }
}