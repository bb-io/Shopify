using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StoreThemeSettingsDataSection;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeSettingsDataSection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme settings data sections")]
public class StoreThemeSettingsDataSectionActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeSettingsDataSection;
    
    [Action("Search store theme settings data sections", Description = "Search store theme settings data sections with specific criteria")]
    public async Task<SearchStoreThemeSettingsDataSectionsResponse> SearchStoreThemeSettingsDataSections(
        [ActionParameter] SearchStoreThemeSettingsDataSectionsRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeSettingsDataSectionResponse(x)).ToList());
    }

    [Action("Download store theme settings data section", Description = "Download content of a specific store theme settings data section")]
    public async Task<FileResponse> DownloadStoreThemeSettingsDataSections(
        [ActionParameter] StoreThemeSettingsDataSectionsIdentifier themeSettingsDataSectionsIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = themeSettingsDataSectionsIdentifier.StoreThemeSettingsDataSectionsId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme settings data sections", Description = "Upload content of a specific store theme settings data section")]
    public Task UploadStoreThemeSettingsDataSections(
        [ActionParameter] UploadStoreThemeSettingsDataSectionRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeSettingsDataSectionId, locale.Locale, marketIdentifier.MarketId);
    }
}