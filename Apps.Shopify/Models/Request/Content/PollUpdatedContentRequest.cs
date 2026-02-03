using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Content;

public class PollUpdatedContentRequest
{
    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Content types"), StaticDataSource(typeof(PollingContentTypeDataHandler))]
    public IEnumerable<string>? ContentTypes { get; set; }
}
