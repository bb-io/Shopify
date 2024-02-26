using Apps.Shopify.Constants;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Shopify.Api;

public class ShopifyClient : BlackBirdRestClient
{
    private const string ApiVersion = "2024-01";
    public ShopifyClient(AuthenticationCredentialsProvider[] creds) : base(new()
    {
        BaseUrl = $"https://{creds.Get(CredsNames.StoreName).Value}.myshopify.com/admin/api/{ApiVersion}".ToUri()
    })
    {
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response.Content!)!;
        return new(errorResponse.Errors);
    }
}