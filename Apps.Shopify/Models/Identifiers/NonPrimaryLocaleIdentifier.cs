using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class NonPrimaryLocaleIdentifier
{
    [DataSource(typeof(NonPrimaryLanguageDataHandler))]
    public string Locale { get; set; }
}