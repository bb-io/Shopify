using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeJsonTemplate;

public class SearchStoreThemeJsonTemplatesRequest
{
    [Display("Email template name contains")]
    public string? NameContains { get; set; }
}