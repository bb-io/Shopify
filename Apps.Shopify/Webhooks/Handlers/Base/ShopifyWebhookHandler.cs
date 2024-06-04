using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Response.Event;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using GraphQL;

namespace Apps.Shopify.Webhooks.Handlers.Base;

public abstract class ShopifyWebhookHandler : ShopifyInvocable, IWebhookEventHandler
{
    protected abstract string Topic { get; }
    
    public ShopifyWebhookHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
    {
        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.SubscribeEvent,
            Variables = new
            {
                topic = Topic,
                webhookSubscription = new
                {
                    callbackUrl = values["payloadUrl"],
                    format = "JSON"
                }
            }
        };
        
        return Client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider, Dictionary<string, string> values)
    {
        var allWebhooks = await GetAllWebhooks();
        var webhookToDelete = allWebhooks.FirstOrDefault(x => x.CallbackUrl == values["payloadUrl"]);

        if (webhookToDelete is null)
            return;
        
        var request = new GraphQLRequest()
        {
            Query = GraphQlMutations.UnsubscribeEvent,
            Variables = new
            {
                id = webhookToDelete.Id
            }
        };
        
        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<List<EventEntity>> GetAllWebhooks()
    {
        return Client.Paginate<EventEntity, EventPaginationResponse>(GraphQlQueries.Events, new());
    }
}