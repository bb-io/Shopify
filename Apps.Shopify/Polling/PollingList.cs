using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;

namespace Apps.Shopify.Polling;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    [PollingEvent("On content created or updated", "On new content is created or existing content is updated")]
    public async Task<PollingEventResponse<DateMemory, ContentCreatedOrUpdatedResponse>> OnContentCreatedOrUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnlineStoreBlogRequest blog)
    {
        var lastInteractionDate = request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz");

        var articlesCreatedTask = HandleArticlePolling(request, blog, $"created_at_min={lastInteractionDate}");
        var articlesUpdatedTask = HandleArticlePolling(request, blog, $"updated_at_min={lastInteractionDate}");
        var pagesCreatedTask = HandlePagesPolling(request, $"created_at_min={lastInteractionDate}");
        var pagesUpdatedTask = HandlePagesPolling(request, $"updated_at_min={lastInteractionDate}");

        await Task.WhenAll(articlesCreatedTask, articlesUpdatedTask, pagesCreatedTask, pagesUpdatedTask);

        var articlesResponse = (await articlesCreatedTask).Result?.Articles.ToList() ?? [];
        articlesResponse.AddRange((await articlesUpdatedTask).Result?.Articles.ToList() ?? []);
        articlesResponse = articlesResponse.Distinct().ToList();

        var content = new List<ContentItemEntity>();
        content.AddRange(articlesResponse
            .Select(a => new ContentItemEntity(a.Id, HtmlMetadataConstants.OnlineStoreArticle, a.Title)).ToList());

        var pagesResponse = (await pagesCreatedTask).Result?.Pages.ToList() ?? [];
        pagesResponse.AddRange((await pagesUpdatedTask).Result?.Pages.ToList() ?? []);
        pagesResponse = pagesResponse.Distinct().ToList();

        content.AddRange(pagesResponse
            .Select(a => new ContentItemEntity(a.Id, HtmlMetadataConstants.OnlineStorePageContent, a.Title)).ToList());

        var response = new ContentCreatedOrUpdatedResponse(content);
        return new PollingEventResponse<DateMemory, ContentCreatedOrUpdatedResponse>
        {
            FlyBird = response.Content.Count != 0,
            Result = response,
            Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
        };
    }

    [PollingEvent("On articles created", "On new articles are created")]
    public Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> OnArticlesCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnlineStoreBlogRequest blog) => HandleArticlePolling(request, blog,
        $"created_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On articles updated", "On any articles are updated")]
    public Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnlineStoreBlogRequest blog) => HandleArticlePolling(request, blog,
        $"updated_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On pages created", "On new pages are created")]
    public Task<PollingEventResponse<DateMemory, SearchPagesResponse>> OnPagesCreated(
        PollingEventRequest<DateMemory> request) => HandlePagesPolling(request,
        $"created_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On pages updated", "On any pages are updated")]
    public Task<PollingEventResponse<DateMemory, SearchPagesResponse>> OnPagesUpdated(
        PollingEventRequest<DateMemory> request) => HandlePagesPolling(request,
        $"updated_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    private async Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> HandleArticlePolling(
        PollingEventRequest<DateMemory> request, OnlineStoreBlogRequest blog, string query)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        var variables = new Dictionary<string, object>
        {
            ["query"] = $"blog_id:{blog.OnlineStoreBlogId.GetShopifyItemId()}"
        };
        var articlesResponse = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            variables
        );
        var result = articlesResponse.Select(x => new GetArticleResponse(x));

        if (!articlesResponse.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        return new()
        {
            FlyBird = true,
            Result = new(result),
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }

    private async Task<PollingEventResponse<DateMemory, SearchPagesResponse>> HandlePagesPolling(
        PollingEventRequest<DateMemory> request, string query)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        var pages = await Client.Paginate<PageEntity, PagesPaginationResponse>(GraphQlQueries.Pages, []);

        if (pages.Count == 0)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        return new()
        {
            FlyBird = true,
            Result = new(pages),
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
}