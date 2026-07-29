using Apps.Shopify.DataSourceHandlers.Static;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Filters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models.Request.Content;

public class SearchContentRequest : ICreatedDateFilter, IUpdatedDateFilter, IPublishedDateFilter
{
    [Display("Content types"), StaticDataSource(typeof(ContentTypeDataHandler))]
    public IEnumerable<string>? ContentTypes { get; set; }

    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Published after")]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    public DateTime? PublishedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    public void Validate()
    {
        this.ValidateDates();
    }
}
