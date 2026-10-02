using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Filter;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Filter;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Filters")]
public class FilterActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Filter;
    
    [Action("Search filters", Description = "Search filters with specific criteria")]
    public async Task<SearchFiltersResponse> SearchFilters([ActionParameter] SearchFiltersRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new FilterResponse(x)).ToList());
    }

    [Action("Download filter", Description = "Download content of a specific filter")]
    public async Task<FileResponse> DownloadFilter(
        [ActionParameter] FilterIdentifier filterIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = filterIdentifier.FilterId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload filter", Description = "Upload content of a specific filter")]
    public Task UploadFilter(
        [ActionParameter] UploadFilterRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.FilterId, locale.Locale, marketIdentifier.MarketId);
    }
}