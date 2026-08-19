using System.Net.Mime;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Menu;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Services.Concrete;

public class MenuService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService
{
    private readonly TranslatableResourceService _resourceService = new(invocationContext, fileManagementClient);
    private readonly string _contentType = TranslatableResources.Menu;
    
    public async Task<FileReference> Download(DownloadContentRequest input)
    {
        var resourceIdsRequest = new GraphQLRequest
        {
            Query = GraphQlQueries.Menu,
            Variables = new
            {
                resourceId = input.ContentId
            }
        };

        var resourceIdsResponse = await Client.ExecuteWithErrorHandling<MenuApiResponse>(resourceIdsRequest);
        var resourceIds = new[] { input.ContentId }
            .Concat(Flatten(resourceIdsResponse.Menu.Items).Select(x => x.Id.ToLinkGid()))
            .ToList();
        
        var resources = await GetTranslatableResources(resourceIds, input);

        var byId = resources.ToDictionary(x => x.ResourceId);
        var entities = resourceIds
            .Where(byId.ContainsKey)
            .SelectMany(id => byId[id].GetTranslatableContent().Select(c => new IdentifiedContentEntity(c) { Id = id }))
            .ToList();

        var metadata = new ShopifyMetadata
        {
            MarketId = input.MarketId,
            ContentType = _contentType
        };
        var htmlStream = ShopifyHtmlConverter.ToHtml(entities, metadata);
        
        return await fileManagementClient.UploadAsync(htmlStream, MediaTypeNames.Text.Html, input.ContentId.GetFileName(input.MarketId));
    }

    public async Task Upload(UploadContentRequest input)
    {
        string html = await HtmlFileHelper.GetHtmlFromFile(fileManagementClient, input.Content);
        var items = ShopifyHtmlConverter.ToJson(html, input.Locale, new ShopifyMetadata { MarketId = input.MarketId }).ToList();

        await _resourceService.UpdateIdentifiedContent(items, null, input.MarketId);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .Build();
        
        var response = await Client.Paginate<MenuEntity, MenusPaginationResponse>(
            GraphQlQueries.Menus,
            QueryHelper.QueryToDictionary(query));

        var contentItems = response.Select(x => new ContentItemEntity(x.Id, _contentType, x.Title)).ToList();
        return new(contentItems);
    }

    private Task<List<TranslatableResourceEntity>> GetTranslatableResources(
        ICollection<string> resourceIds, 
        DownloadContentRequest input)
    {
        var variables = new Dictionary<string, object>
        {
            ["resourceIds"] = resourceIds,
            ["locale"] = input.Locale,
            ["outdated"] = input.Outdated ?? false
        };

        if (!string.IsNullOrEmpty(input.MarketId))
            variables["marketId"] = input.MarketId;

        return Client.Paginate<TranslatableResourceEntity, TranslatableResourcesByIdsPaginationResponse>(
            GraphQlQueries.TranslatableResourcesByIds, 
            variables);
    }

    private static IEnumerable<MenuItemEntity> Flatten(IEnumerable<MenuItemEntity> items)
    {
        return items.SelectMany(x => new[] { x }.Concat(Flatten(x.Items)));
    }
}