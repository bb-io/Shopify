using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.SellingPlan;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.SellingPlan;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Selling plans")]
public class SellingPlanActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.SellingPlan;
    
    [Action("Search selling plans", Description = "Search selling plans with specific criteria")]
    public async Task<SearchSellingPlansResponse> SearchSellingPlans(
        [ActionParameter] SearchSellingPlansRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new SellingPlanResponse(x)).ToList());
    }

    [Action("Download selling plan", Description = "Download content of a specific selling plan")]
    public async Task<FileResponse> DownloadSellingPlan(
        [ActionParameter] SellingPlanIdentifier sellingPlanIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = sellingPlanIdentifier.SellingPlanId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload selling plan", Description = "Upload content of a specific selling plan")]
    public Task UploadSellingPlan(
        [ActionParameter] UploadSellingPlanRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.SellingPlanId, locale.Locale, marketIdentifier.MarketId);
    }
}