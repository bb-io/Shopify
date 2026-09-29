using Apps.Shopify.Constants;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Polling.Models.Memory;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Polling;

[PollingEventList("Content")]
public class ContentPollingList(InvocationContext invocationContext) : ShopifyInvocable(invocationContext)
{
    private readonly ContentServiceFactory _factory = new(invocationContext);

    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [MultipleEvents, PollingEvent("On content updated", "Triggered when a content item is created or updated")]
    public async Task<PollingEventResponse<DigestDateMemory, List<PollingContentItemEntity>>> OnContentUpdated(
        PollingEventRequest<DigestDateMemory> request,
        [PollingEventParameter] PollUpdatedContentRequest input)
    {
        var now = DateTime.UtcNow;
        if (request.Memory?.LastInteractionDate is null)
        {
            var seed = new Dictionary<string, string>();
            var digestContentTypes = input.ContentTypes ?? TranslatableResources.SupportedPollingContentTypes;
            
            foreach (var service in _factory.GetDigestPollingContentServices(digestContentTypes))
            {
                var result = await service.PollUpdated(seed, input);
                foreach (var (key, value) in result.Digests) 
                    seed[key] = value;
            }

            return new()
            {
                FlyBird = false, 
                Result = null,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow, 
                    Digests = seed
                }
            };
        }

        var lastInteractionDate = request.Memory.LastInteractionDate.Value;

        input.ContentTypes ??= TranslatableResources.SupportedPollingContentTypes;
        var services = _factory.GetPollingContentServices(input.ContentTypes);

        var allItems = new List<PollingContentItemEntity>();
        foreach (var service in services)
        {
            var items = await service.PollUpdated(lastInteractionDate, now, input);
            allItems.AddRange(items.Items);
        }
        
        var digestServices = _factory.GetDigestPollingContentServices(input.ContentTypes);
        var digests = new Dictionary<string, string>(request.Memory.Digests);

        foreach (var service in digestServices)
        {
            var result = await service.PollUpdated(request.Memory.Digests, input);
            allItems.AddRange(result.Items);
            foreach (var (key, value) in result.Digests) digests[key] = value;
        }

        return new()
        {
            FlyBird = allItems.Count != 0,
            Result = allItems,
            Memory = new() { LastInteractionDate = now, Digests = digests }
        };
    }
}
