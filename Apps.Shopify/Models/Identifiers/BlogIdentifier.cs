using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class BlogIdentifier
{
    [Display("Blog ID")]
    [DataSource(typeof(BlogDataHandler))]
    public string BlogId { get; set; }
}