using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Menu;
using Apps.Shopify.Models.Entities.Polling;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Models.Response.Menu;
using Apps.Shopify.Models.Response.TranslatableResource;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using GraphQL;

namespace Apps.Shopify.Services.Concrete;

public class MenuService(InvocationContext invocationContext)
    : BaseContentService(invocationContext), IContentService, IDigestPollingContentService
{
    protected override string ContentType => TranslatableResources.Menu;

    public async Task<FileRecord> Download(DownloadContentRequest input)
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
            ContentType = ContentType
        };
        var htmlStream = ShopifyHtmlConverter.ToHtml(entities, metadata);
        
        return new FileRecord(htmlStream, MediaTypeNames.Text.Html, input.ContentId.GetFileName(input.MarketId));
    }

    public async Task Upload(UploadContentServiceRequest input)
    {
        var metadata = new ShopifyMetadata { MarketId = input.MarketId };
        var items = ShopifyHtmlConverter.ToJson(input.HtmlContent, input.Locale, metadata).ToList();

        await ResourceService.UpdateIdentifiedContent(items, null, input.MarketId);
    }

    public async Task<SearchContentResponse> Search(SearchContentRequest input)
    {
        string? query = new QueryBuilder()
            .AddContains("title", input.NameContains)
            .Build();
        
        var response = await Client.Paginate<MenuEntity, MenusPaginationResponse>(
            GraphQlQueries.Menus,
            QueryHelper.QueryToDictionary(query));

        var contentItems = response.Select(x => new ContentItemEntity(x.Id, ContentType, x.Title)).ToList();
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

        var items = menus.Select(m => new DigestItemEntity(m.Id, m.Title, ComputeMenuHash(m, digests, linkDigests)));
        return BuildDigestPollResult(knownDigests, items);
    }

    private async Task<Dictionary<string, string>> GetDigests(TranslatableResource resourceType)
    {
        var resources = await ListTranslatableResources(resourceType);
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