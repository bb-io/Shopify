using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class LocaleIdentifier
{
    [Display("Locale"), DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }
}