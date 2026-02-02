using Apps.Shopify.Constants;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Polling.Models.Memory;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;

namespace Apps.Shopify.Polling;

[PollingEventList("Content")]
public class ContentPollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext, null!);

    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    [PollingEvent("On content updated", "On existing content is updated")]
    public async Task<PollingEventResponse<DateMemory, ContentUpdatedResponse>> OnContentUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollUpdatedContentRequest input)
    {
        if (request.Memory is null || request.Memory.LastInteractionDate is null)
        {
            return new PollingEventResponse<DateMemory, ContentUpdatedResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = null
            };
        }

        var lastInteractionDate = request.Memory.LastInteractionDate.Value;
        var now = DateTime.UtcNow;

        input.ContentTypes ??= TranslatableResources.SupportedPollingContentTypes;
        var services = _factory.GetPollingContentServices(input.ContentTypes);

        var allItems = new List<PollingContentItemEntity>();
        foreach (var service in services)
        {
            var items = await service.PollUpdated(lastInteractionDate, now, input);
            allItems.AddRange(items.Items);
        }

        return new()
        {
            FlyBird = allItems.Count != 0,
            Result = new(allItems),
            Memory = new() { LastInteractionDate = now }
        };
    }
}
