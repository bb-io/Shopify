using Apps.Shopify.Models.Filters;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.Collection;

public class SearchCollectionsRequest : IUpdatedDateFilter
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Product IDs")]
    public IEnumerable<string>? ProductIds { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }
}
