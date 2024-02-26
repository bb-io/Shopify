using Apps.Shopify.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Shopify.Api;

public class ShopifyRequest : BlackBirdRestRequest
{
    public ShopifyRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) : base(
        resource, method, creds)
    {
    }

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var token = creds.Get(CredsNames.Token).Value;
        this.AddHeader("X-Shopify-Access-Token", token);
    }
}