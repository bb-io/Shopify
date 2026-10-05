using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.StoreThemeSectionGroup;

public record SearchStoreThemeSectionGroupsResponse(List<StoreThemeSectionGroupResponse> SectionGroups)
{
    [Display("Section groups")]
    public List<StoreThemeSectionGroupResponse> SectionGroups { get; set; } = SectionGroups;
}