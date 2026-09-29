using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.EmailTemplate;

public class SearchEmailTemplatesRequest
{
    [Display("Email template name contains")]
    public string? NameContains { get; set; }
}