using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Content;

public class ContentTypeIdentifier
{
    [Display("Content type"), StaticDataSource(typeof(ContentTypeDataHandler))]
    public string ContentType { get; set; }
}
