using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StoreThemeLocaleContent;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeLocaleContent;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme locale content")]
public class StoreThemeLocaleContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeLocaleContent;
    
    [Action("Search store theme locale content", Description = "Search store theme locale content with specific criteria")]
    public async Task<SearchStoreThemeLocaleContentResponse> SearchStoreThemeLocaleContent(
        [ActionParameter] SearchStoreThemeLocaleContentRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeLocaleContentResponse(x)).ToList());
    }

    [Action("Download store theme locale content", Description = "Download content of a specific store theme locale content")]
    public async Task<FileResponse> DownloadStoreThemeLocaleContent(
        [ActionParameter] StoreThemeLocaleContentIdentifier themeLocaleContentIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = themeLocaleContentIdentifier.StoreThemeLocaleContentId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme locale content", Description = "Upload content of a specific store theme locale content")]
    public Task UploadStoreThemeLocaleContent(
        [ActionParameter] UploadStoreThemeLocaleContentRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeLocaleContentId, locale.Locale, marketIdentifier.MarketId);
    }
}