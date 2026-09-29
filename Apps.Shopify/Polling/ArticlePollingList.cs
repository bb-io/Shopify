using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Extensions;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Identifiers;
using Apps.Shopify.Models.Response.Article;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Shopify.Polling;

[PollingEventList("Articles")]
public class ArticlePollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On article created", "Triggered when an article is created")]
    public Task<PollingEventResponse<DateMemory, List<GetArticleResponse>>> OnArticlesCreated(
        PollingEventRequest<DateMemory> request, 
        [PollingEventParameter] BlogIdentifier blog) =>
        HandlePolling(request, blog, isCreatedMode: true);

    [MultipleEvents, PollingEvent("On article updated", "Triggered when an article is updated")]
    public Task<PollingEventResponse<DateMemory, List<GetArticleResponse>>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request, 
        [PollingEventParameter] BlogIdentifier blog) =>
        HandlePolling(request, blog, isCreatedMode: false);

    private async Task<PollingEventResponse<DateMemory, List<GetArticleResponse>>> HandlePolling(
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
            Result = result,
            Memory = new() { LastInteractionDate = now }
        };
    }
}
