using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class EmailTemplateIdentifier
{
    [Display("Email template ID"), DataSource(typeof(EmailTemplateDataHandler))]
    public string EmailTemplateId { get; set; } = string.Empty;
}