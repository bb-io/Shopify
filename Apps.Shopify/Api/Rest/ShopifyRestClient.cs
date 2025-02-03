using Apps.Shopify.Constants;
using Apps.Shopify.Extensions;
using Apps.Shopify.Models.Response;
using Apps.Shopify.Models.Response.Pagination;
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

    public async Task<List<T>> Paginate<T, TV>(RestRequest request) where TV : IRestPaginationResponse<T>
        where T : IRestPaginationEntity
    {
        var limit = 250;
        var lastId = string.Empty;
        var baseUrl = request.Resource;

        IRestPaginationResponse<T> response;
        var result = new List<T>();

        do
        {
            request.Resource = baseUrl
                .SetQueryParameter("limit", limit.ToString());
            
            if(!string.IsNullOrWhiteSpace(lastId))
                request.Resource = request.Resource.SetQueryParameter("since_id", lastId);

            response = await ExecuteWithErrorHandling<TV>(request);
            result.AddRange(response.Items);

            lastId = response.Items.OrderByDescending(x => long.Parse(x.Id.GetShopifyItemId())).FirstOrDefault()?.Id.GetShopifyItemId();
        } while (response.Items.Any());

        return result;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<RestErrorResponse>(response.Content!)!;
        return new PluginApplicationException(error.Errors);
    }

    public static string GenerateApiUrl(AuthenticationCredentialsProvider[] creds, string apiVersion) =>
        $"https://{creds.Get(CredsNames.StoreName).Value}.myshopify.com/admin/api/{apiVersion}";
}