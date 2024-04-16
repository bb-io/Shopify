using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request;

public class NonPrimaryLocaleRequest
{
    [DataSource(typeof(NonPrimaryLanguageDataHandler))]
    public string Locale { get; set; }
}