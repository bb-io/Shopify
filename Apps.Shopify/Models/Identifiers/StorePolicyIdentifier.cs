using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class StorePolicyIdentifier
{
    [Display("Store policy ID"), DataSource(typeof(StorePolicyDataHandler))]
    public string StorePolicyId { get; set; } = string.Empty;
}