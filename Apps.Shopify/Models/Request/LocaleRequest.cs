using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Request;

public class LocaleRequest
{
    [DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }
}