using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class LocaleIdentifier
{
    [DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }
}