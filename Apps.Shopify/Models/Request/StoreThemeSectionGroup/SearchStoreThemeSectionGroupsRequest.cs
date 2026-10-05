using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.StoreThemeSectionGroup;

public class SearchStoreThemeSectionGroupsRequest
{
    [Display("Store theme section group name contains")]
    public string? NameContains { get; set; }
}