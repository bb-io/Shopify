using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Shopify.Polling;

[PollingEventList("Pages")]
public class PagePollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [PollingEvent("On pages created", "On new pages are created")]
    public Task<PollingEventResponse<DateMemory, SearchPagesResponse>> OnPagesCreated(
        PollingEventRequest<DateMemory> request) => HandlePolling(request, isCreatedMode: true);

    [PollingEvent("On pages updated", "On any pages are updated")]
    public Task<PollingEventResponse<DateMemory, SearchPagesResponse>> OnPagesUpdated(
        PollingEventRequest<DateMemory> request) => HandlePolling(request, isCreatedMode: false);

    private async Task<PollingEventResponse<DateMemory, SearchPagesResponse>> HandlePolling(
        PollingEventRequest<DateMemory> request, 
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
            .Build();

        var response = await Client.Paginate<PageEntity, PagesPaginationResponse>(
            GraphQlQueries.Pages,
            QueryHelper.QueryToDictionary(query)
        );

        return new()
        {
            FlyBird = response.Count > 0,
            Result = new(response),
            Memory = new() { LastInteractionDate = now }
        };
    }
}