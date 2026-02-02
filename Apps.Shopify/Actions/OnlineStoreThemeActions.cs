using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.OnlineStoreTheme;
using Apps.Shopify.Models.Request.Theme;
using Apps.Shopify.Models.Response.Theme;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Themes")]
public class OnlineStoreThemeActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search themes", Description = "Search themes with specific criteria")]
    public async Task<SearchThemesResponse> SearchThemes([ActionParameter] SearchThemesRequest input)
    {
        var variables = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(input.Role))
            variables["roles"] = new[] { input.Role };

        var response = await Client.Paginate<ThemeEntity, ThemesPaginationResponse>(
            GraphQlQueries.Themes,
            variables
        );

        if (!string.IsNullOrEmpty(input.NameContains))
            response = response.Where(x => x.Name.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();

        return new(response);
    }

    [Action("Download theme", Description = "Download content of a specific theme")]
    public async Task<DownloadThemeResponse> GetOnlineStoreThemeTranslationContent(
        [ActionParameter] ThemeIdentifier theme,
        [ActionParameter] DownloadThemeRequest input,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.ONLINE_STORE_THEME);
        var request = new DownloadContentRequest
        {
            ContentId = theme.ThemeId,
            AssetKeys = input.AssetKeys,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload theme", Description = "Upload content of a specific theme")]
    public async Task UpdateOnlineStoreThemeContent(
        [ActionParameter] UploadThemeRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale)
    {
        var service = _factory.GetContentService(TranslatableResource.ONLINE_STORE_THEME);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.ThemeId,
            Locale = locale.Locale
        };

        await service.Upload(request);
    }
}