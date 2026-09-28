using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.EmailTemplate;

public record SearchEmailTemplatesResponse(List<EmailTemplateResponse> Templates)
{
    [Display("Email templates")]
    public List<EmailTemplateResponse> Templates { get; set; } = Templates;
}