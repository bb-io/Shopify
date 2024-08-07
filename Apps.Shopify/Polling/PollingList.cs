using Apps.Shopify.Api.Rest;
using Apps.Shopify.Extensions;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Request.OnlineStoreBlog;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Shopify.Polling;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [PollingEvent("On articles created", "On new articles are created")]
    public Task<PollingEventResponse<DateMemory, ArticlesPaginationResponse>> OnArticlesCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnlineStoreBlogRequest blog) => HandleArticlePolling(request, blog,
        $"created_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On articles updated", "On any articles are updated")]
    public Task<PollingEventResponse<DateMemory, ArticlesPaginationResponse>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnlineStoreBlogRequest blog) => HandleArticlePolling(request, blog,
        $"updated_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On pages created", "On new pages are created")]
    public Task<PollingEventResponse<DateMemory, PagesPaginationResponse>> OnPagesCreated(
        PollingEventRequest<DateMemory> request) => HandlePagesPolling(request,
        $"created_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    [PollingEvent("On pages updated", "On any pages are updated")]
    public Task<PollingEventResponse<DateMemory, PagesPaginationResponse>> OnPagesUpdated(
        PollingEventRequest<DateMemory> request) => HandlePagesPolling(request,
        $"updated_at_min={request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:sszzz")}");

    private async Task<PollingEventResponse<DateMemory, ArticlesPaginationResponse>> HandleArticlePolling(
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

        var articlesRequest =
            new ShopifyRestRequest(
                $"blogs/{blog.OnlineStoreBlogId.GetShopifyItemId()}/articles.json?{query}",
                Method.Get, Creds);
        var items = await new ShopifyRestClient(Creds)
            .Paginate<OnlineStoreArticleEntity, ArticlesPaginationResponse>(articlesRequest);

        if (!items.Any())
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
            Result = new()
            {
                Items = items
            },
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }

    private async Task<PollingEventResponse<DateMemory, PagesPaginationResponse>> HandlePagesPolling(
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

        var articlesRequest = new ShopifyRestRequest($"pages.json?{query}", Method.Get, Creds);
        var items = await new ShopifyRestClient(Creds)
            .Paginate<OnlineStorePageEntity, PagesPaginationResponse>(articlesRequest);

        if (!items.Any())
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
            Result = new()
            {
                Items = items
            },
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
}