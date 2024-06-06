using Apps.Shopify.Api;
using Apps.Shopify.Api.Rest;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Invocables;

public class ShopifyInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected ShopifyRestClient RestClient { get; }
    protected ShopifyClient Client { get; }
    public ShopifyInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
        RestClient = new(Creds);
    }
}