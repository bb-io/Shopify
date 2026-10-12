using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.SellingPlanGroup;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.SellingPlanGroup;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Selling plan groups")]
public class SellingPlanGroupActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.SellingPlanGroup;
    
    [Action("Search selling plan groups", Description = "Search selling plan groups with specific criteria")]
    public async Task<SearchSellingPlanGroupsResponse> SearchSellingPlanGroups(
        [ActionParameter] SearchSellingPlanGroupsRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new SellingPlanGroupResponse(x)).ToList());
    }

    [Action("Download selling plan group", Description = "Download content of a specific selling plan group")]
    public async Task<FileResponse> DownloadSellingPlanGroup(
        [ActionParameter] SellingPlanGroupIdentifier sellingPlanGroupIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = sellingPlanGroupIdentifier.SellingPlanGroupId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload selling plan group", Description = "Upload content of a specific selling plan group")]
    public Task UploadSellingPlanGroup(
        [ActionParameter] UploadSellingPlanGroupRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.SellingPlanGroupId, locale.Locale, marketIdentifier.MarketId);
    }
}