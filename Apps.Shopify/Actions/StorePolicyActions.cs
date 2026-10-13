using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StorePolicy;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StorePolicy;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store policies")]
public class StorePolicyActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StorePolicy;
    
    [Action("Search store policies", Description = "Search store policies with specific criteria")]
    public async Task<SearchStorePoliciesResponse> SearchStorePolicies(
        [ActionParameter] SearchStorePoliciesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StorePolicyResponse(x)).ToList());
    }

    [Action("Download store policy", Description = "Download content of a specific store policy")]
    public async Task<FileResponse> DownloadStorePolicy(
        [ActionParameter] StorePolicyIdentifier storePolicyIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = storePolicyIdentifier.StorePolicyId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store policy", Description = "Upload content of a specific store policy")]
    public Task UploadStorePolicy(
        [ActionParameter] UploadStorePolicyRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.StorePolicyId, locale.Locale, marketIdentifier.MarketId);
    }
}