using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Page;
using Apps.Shopify.Models.Response.Page;
using Apps.Shopify.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Shopify.Polling;

[PollingEventList("Pages")]
public class PagePollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On page created", "Triggered when a new page is created")]
    public Task<PollingEventResponse<DateMemory, List<PageEntity>>> OnPagesCreated(
        PollingEventRequest<DateMemory> request) => HandlePolling(request, isCreatedMode: true);

    [MultipleEvents, PollingEvent("On page updated", "Triggered when a new page is updated")]
    public Task<PollingEventResponse<DateMemory, List<PageEntity>>> OnPagesUpdated(
        PollingEventRequest<DateMemory> request) => HandlePolling(request, isCreatedMode: false);

    private async Task<PollingEventResponse<DateMemory, List<PageEntity>>> HandlePolling(
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
            Result = response,
            Memory = new() { LastInteractionDate = now }
        };
    }
}