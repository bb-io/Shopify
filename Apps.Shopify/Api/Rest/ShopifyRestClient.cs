using Apps.Shopify.Constants;
using Apps.Shopify.Models.Response;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Shopify.Api.Rest;

public class ShopifyRestClient : BlackBirdRestClient
{
    protected override JsonSerializerSettings? JsonSettings => JsonConfig.RestJsonSettings;

    public ShopifyRestClient(AuthenticationCredentialsProvider[] creds) : base(new()
    {
        BaseUrl = GenerateApiUrl(creds, ApiConstants.ApiVersion).ToUri()
    })
    {
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<RestErrorResponse>(response.Content!)!;
        return new PluginApplicationException(error.Errors);
    }

    public static string GenerateApiUrl(AuthenticationCredentialsProvider[] creds, string apiVersion) =>
        $"https://{creds.Get(CredsNames.StoreName).Value}.myshopify.com/admin/api/{apiVersion}";
}