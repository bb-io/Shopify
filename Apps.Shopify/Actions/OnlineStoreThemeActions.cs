using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Theme;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Request;
using Apps.Shopify.Models.Request.Assets;
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

[ActionList("Online store themes")]
public class OnlineStoreThemeActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, fileManagementClient);

    [Action("Search online store themes", Description = "Search online store themes with specific criteria")]
    public async Task<SearchThemesResponse> SearchThemes([ActionParameter] SearchThemesRequest input)
    {
        var variables = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(input.Role))
            variables["roles"] = new[] { input.Role };

        var response = await Client.Paginate<ThemeEntity, ThemesPaginationResponse>(
            GraphQlQueries.Themes,
            variables
        );

        return new(response);
    }

    [Action("Download online store theme", Description = "Get content of a specific online store theme")]
    public async Task<DownloadThemeResponse> GetOnlineStoreThemeTranslationContent(
        [ActionParameter] GetOnlineStoreThemeContentAsHtmlRequest input, 
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] GetContentRequest getContentRequest)
    {
        var service = _factory.GetContentService(TranslatableResource.ONLINE_STORE_THEME);
        var request = new DownloadContentRequest
        {
            ContentId = input.OnlineStoreThemeId,
            AssetKeys = input.AssetKeys,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
        };

        var file = await service.Download(request);
        return new(file);
    }

    [Action("Upload online store theme", Description = "Update content of a specific online store theme")]
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