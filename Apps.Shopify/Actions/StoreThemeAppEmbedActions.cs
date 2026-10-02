using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StoreThemeAppEmbed;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeAppEmbed;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme app embeds")]
public class StoreThemeAppEmbedActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeAppEmbed;
    
    [Action("Search store theme app embeds", Description = "Search store theme app embeds with specific criteria")]
    public async Task<SearchStoreThemeAppEmbedsResponse> SearchStoreThemeAppEmbeds(
        [ActionParameter] SearchStoreThemeAppEmbedsRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeAppEmbedResponse(x)).ToList());
    }

    [Action("Download store theme app embed", Description = "Download content of a specific store theme app embed")]
    public async Task<FileResponse> DownloadStoreThemeAppEmbed(
        [ActionParameter] StoreThemeAppEmbedIdentifier themeAppEmbedIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = themeAppEmbedIdentifier.StoreThemeAppEmbedId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme app embed", Description = "Upload content of a specific store theme app embed")]
    public Task UploadStoreThemeAppEmbed(
        [ActionParameter] UploadStoreThemeAppEmbedRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeAppEmbedIdId, locale.Locale, marketIdentifier.MarketId);
    }
}