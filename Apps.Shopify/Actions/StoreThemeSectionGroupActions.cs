using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StoreThemeSectionGroup;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeSectionGroup;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme section groups")]
public class StoreThemeSectionGroupActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeSectionGroup;
    
    [Action("Search store theme section groups", Description = "Search store theme section groups with specific criteria")]
    public async Task<SearchStoreThemeSectionGroupsResponse> SearchStoreThemeSectionGroups(
        [ActionParameter] SearchStoreThemeSectionGroupsRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeSectionGroupResponse(x)).ToList());
    }

    [Action("Download store theme section group", Description = "Download content of a specific store theme section group")]
    public async Task<FileResponse> DownloadStoreThemeSectionGroup(
        [ActionParameter] StoreThemeSectionGroupIdentifier themeSectionGroupIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = themeSectionGroupIdentifier.StoreThemeSectionGroupId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme section group", Description = "Upload content of a specific store theme section group")]
    public Task UploadStoreThemeSectionGroup(
        [ActionParameter] UploadStoreThemeSectionGroupRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeSectionGroupId, locale.Locale, marketIdentifier.MarketId);
    }
}