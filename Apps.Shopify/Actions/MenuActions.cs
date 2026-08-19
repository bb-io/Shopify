using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Menu;
using Apps.Shopify.Models.Response.Menu;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Menus")]
public class MenuActions(InvocationContext context, IFileManagementClient fileManagementClient) : ShopifyInvocable(context)
{
    private readonly ContentServiceFactory _factory = new(context, fileManagementClient);
    private readonly string _contentType = TranslatableResources.Menu;
    
    [Action("Search menus", Description = "Search menus with specific criteria")]
    public async Task<SearchMenusResponse> SearchMenus(SearchMenusRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .Build();
        
        var response = await Client.Paginate<MenuEntity, MenusPaginationResponse>(
            GraphQlQueries.Menus,
            QueryHelper.QueryToDictionary(query));

        var menus = response.Select(x => new MenuResponse(x)).ToList();
        return new(menus);
    }

    [Action("Download menu", Description = "Download content of a specific menu")]
    public async Task<DownloadMenuResponse> DownloadMenu(
        [ActionParameter] MenuIdentifier menuIdentifier,
        [ActionParameter] LocaleIdentifier locale,
        [ActionParameter] OutdatedOptionalIdentifier getContentRequest,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var service = _factory.GetContentService(_contentType);
        var request = new DownloadContentRequest
        {
            ContentId = menuIdentifier.MenuId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };

        var file = await service.Download(request);
        return new(file);
    }
    
    [Action("Upload menu", Description = "Upload content of a specific menu")]
    public async Task UploadMenu(
        [ActionParameter] UploadMenuRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        var service = _factory.GetContentService(_contentType);
        var request = new UploadContentRequest
        {
            Content = input.File,
            ContentId = input.MenuId,
            Locale = locale.Locale,
            MarketId = marketIdentifier.MarketId
        };

        await service.Upload(request);
    }
}