using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.StoreThemeSettingsCategory;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.StoreThemeSettingsCategory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Store theme settings category")]
public class StoreThemeSettingsCategoryActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseContentActions(invocationContext, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.StoreThemeSettingsCategory;
    
    [Action("Search theme settings categories", Description = "Search store theme settings categories with specific criteria")]
    public async Task<SearchStoreThemeSettingsCategoriesResponse> SearchStoreThemeSettingsCategories(
        [ActionParameter] SearchStoreThemeSettingsCategoriesRequest input)
    {
        var searchInput = new SearchContentRequest { NameContains = input.NameContains };
        var items = await ContentService.Search(searchInput);
        return new(items.Items.Select(x => new StoreThemeSettingsCategoryResponse(x)).ToList());
    }

    [Action("Download store theme settings category", Description = "Download content of a specific store theme settings category")]
    public async Task<FileResponse> DownloadStoreThemeSettingsCategory(
        [ActionParameter] StoreThemeSettingsCategoryIdentifier themeSettingsCategoryIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var request = new DownloadContentRequest
        {
            ContentId = themeSettingsCategoryIdentifier.StoreThemeSettingsCategoryId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload store theme settings category", Description = "Upload content of a specific store theme settings category")]
    public Task UploadStoreThemeSettingsCategory(
        [ActionParameter] UploadStoreThemeSettingsCategoryRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.ThemeSettingsCategoryId, locale.Locale, marketIdentifier.MarketId);
    }
}