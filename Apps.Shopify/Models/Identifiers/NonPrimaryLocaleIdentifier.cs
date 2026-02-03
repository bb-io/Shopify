using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class NonPrimaryLocaleIdentifier
{
    [Display("Non-primary locale"), DataSource(typeof(NonPrimaryLanguageDataHandler))]
    public string Locale { get; set; }
}