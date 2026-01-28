using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Collection;

public class CollectionEntity
{
    [Display("ID")]
    public string Id { get; set; }

    [Display("Title")]
    public string Title { get; set; }

    [Display("Description")]
    public string Description { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }
}
