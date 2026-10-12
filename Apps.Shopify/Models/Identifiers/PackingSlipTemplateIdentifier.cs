using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class PackingSlipTemplateIdentifier
{
    [Display("Packing slip template ID"), DataSource(typeof(PackingSlipTemplateDataHandler))]
    public string PackingSlipTemplateId { get; set; } = string.Empty;
}