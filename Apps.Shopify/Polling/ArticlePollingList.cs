using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Shopify.Polling;

[PollingEventList("Articles")]
public class ArticlePollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [PollingEvent("On articles created", "On new articles are created")]
    public Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> OnArticlesCreated(
        PollingEventRequest<DateMemory> request, 
        [PollingEventParameter] BlogIdentifier blog) =>
        HandlePolling(request, blog, isCreatedMode: true);

    [PollingEvent("On articles updated", "On any articles are updated")]
    public Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request, 
        [PollingEventParameter] BlogIdentifier blog) =>
        HandlePolling(request, blog, isCreatedMode: false);

    private async Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> HandlePolling(
        PollingEventRequest<DateMemory> request,
        BlogIdentifier blog,
        bool isCreatedMode)
    {
        if (request.Memory is null || request.Memory.LastInteractionDate is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new() { LastInteractionDate = DateTime.UtcNow }
            };
        }

        var lastDate = request.Memory.LastInteractionDate.Value;
        var now = DateTime.UtcNow;
        var dateField = isCreatedMode ? "created_at" : "updated_at";

        string? query = new QueryBuilder()
             .AddDateRange(dateField, lastDate, now)
             .AddEquals("blog_id", blog.BlogId.GetShopifyItemId())
             .Build();

        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles,
            QueryHelper.QueryToDictionary(query)
        );

        var result = response.Select(x => new GetArticleResponse(x)).ToList();
        return new()
        {
            FlyBird = result.Count > 0,
            Result = new(result),
            Memory = new() { LastInteractionDate = now }
        };
    }
}
