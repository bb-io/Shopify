using Apps.Shopify.Actions.Base;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Identifiers.Optional;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.Menu;
using Apps.Shopify.Models.Response.Menu;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Actions;

[ActionList("Menus")]
public class MenuActions(InvocationContext context, IFileManagementClient fileManagementClient) 
    : BaseContentActions(context, fileManagementClient)
{
    protected override string ContentType => TranslatableResources.Menu;
    
    [Action("Search menus", Description = "Search menus with specific criteria")]
    public async Task<SearchMenusResponse> SearchMenus([ActionParameter] SearchMenusRequest input)
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
        var request = new DownloadContentRequest
        {
            ContentId = menuIdentifier.MenuId,
            Locale = locale.Locale,
            Outdated = getContentRequest.Outdated,
            MarketId = marketIdentifier.MarketId
        };
        
        var file = await DownloadContent(request);
        return new(file);
    }
    
    [Action("Upload menu", Description = "Upload content of a specific menu")]
    public Task UploadMenu(
        [ActionParameter] UploadMenuRequest input,
        [ActionParameter] NonPrimaryLocaleIdentifier locale,
        [ActionParameter] OptionalMarketIdentifier marketIdentifier)
    {
        return UploadContent(input.File, input.MenuId, locale.Locale, marketIdentifier.MarketId);
    }
}