using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
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
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using GraphQL;

namespace Apps.Shopify.Services.Concrete;

public class MenuService(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : ShopifyInvocable(invocationContext), IContentService, IDigestPollingContentService
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

    public async Task<DigestPollResult> PollUpdated(
        IReadOnlyDictionary<string, string> knownDigests,
        PollUpdatedContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .Build();
        var menus = await Client.Paginate<MenuEntity, MenusPaginationResponse>(
            GraphQlQueries.MenusWithItems, 
            QueryHelper.QueryToDictionary(query));

        var digests = await GetDigests(TranslatableResource.MENU);
        var linkDigests = await GetDigests(TranslatableResource.LINK);

        var current = menus.ToDictionary(m => m.Id, m => ComputeMenuHash(m, digests, linkDigests));
        var changed = menus
            .Where(m => !knownDigests.TryGetValue(m.Id, out var known) || known != current[m.Id])
            .Select(m => new PollingContentItemEntity(m.Id, _contentType, m.Title))
            .ToList();

        return new(changed, current);
    }

    private async Task<Dictionary<string, string>> GetDigests(TranslatableResource resourceType)
    {
        var resources = await Client.Paginate<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            new Dictionary<string, object> { ["resourceType"] = resourceType });

        return resources.ToDictionary(x => x.ResourceId, x => x.TranslatableContent.FirstOrDefault()?.Digest ?? string.Empty);
    }

    private static string ComputeMenuHash(
        MenuEntity menu,
        IReadOnlyDictionary<string, string> menuDigests,
        IReadOnlyDictionary<string, string> linkDigests)
    {
        var parts = Flatten(menu.Items)
            .Select(i => linkDigests.GetValueOrDefault(i.Id.ToLinkGid(), string.Empty))
            .Order()    // So that reordering in the UI does not fire a new event
            .Prepend(menuDigests.GetValueOrDefault(menu.Id, string.Empty));

        string joined = string.Join('|', parts);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(joined)));
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