using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.EmailTemplate;

public class UploadEmailTemplateRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Email template"), DataSource(typeof(EmailTemplateDataHandler))]
    public string? EmailTemplateId { get; set; }
}