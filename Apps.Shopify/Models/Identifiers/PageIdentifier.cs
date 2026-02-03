using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class PageIdentifier
{
    [Display("Page ID")]
    [DataSource(typeof(PageDataHandler))]
    public string PageId { get; set; }
}