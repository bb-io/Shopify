using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;

namespace Apps.Shopify.Models.Request.Content;

public class SearchContentRequest
{
    [Display("Content types"), StaticDataSource(typeof(ContentTypeDataHandler))]
    public IEnumerable<string>? ContentTypes { get; set; }

    [Display("Name or title contains")]
    public string? NameContains { get; set; }


}
