using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class FilterIdentifier
{
    [Display("Filter ID"), DataSource(typeof(FilterDataHandler))]
    public string FilterId { get; set; } = string.Empty;
}