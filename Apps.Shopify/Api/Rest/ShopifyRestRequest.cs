using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Shopify.Api.Rest;

public class ShopifyRestRequest : BlackBirdRestRequest
{
    public ShopifyRestRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) : base(resource, method, creds)
    {
    }

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        this.AddHeader("X-Shopify-Access-Token", creds.Get(CredsNames.Token).Value);
    }
}