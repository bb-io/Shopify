using Apps.Shopify.Constants;
using Apps.Shopify.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Api;

public class ShopifyClient : GraphQLHttpClient
{
    public ShopifyClient(AuthenticationCredentialsProvider[] creds, string? endpoint = default) : base(
        endpoint ?? GenerateApiUrl(creds, ApiConstants.ApiVersion),
        new NewtonsoftJsonSerializer())
    {
        var token = creds.Get(CredsNames.Token).Value;
        HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", token);
    }

    public async Task<T> ExecuteWithErrorHandling<T>(GraphQLRequest request,
        CancellationToken cancellationToken = default)
    {
        GraphQLResponse<T>? response = null;

        try
        {
            response = await SendQueryAsync<T>(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new PluginApplicationException($"HTTP error during GraphQL request: {ex.Message}");
        }

        if (response?.Errors is not null && response.Errors.Any())
        {
            var combinedErrorMessage = string.Join("; ", response.Errors.Select(x => x.Message));
            throw new PluginApplicationException(combinedErrorMessage);
        }

        return response!.Data;
    }

    public async Task<GraphQLResponse<JObject>> ExecuteWithErrorHandling(GraphQLRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendQueryAsync<JObject>(request, cancellationToken);

        if (response.Errors is not null && response.Errors.Any())
            throw new PluginApplicationException(string.Join(';', response.Errors.Select(x => x.Message)));

        return response;
    }

    public async Task<List<T>> Paginate<T, TV>(string query, Dictionary<string, object> variables,
        CancellationToken cancellationToken = default)
        where TV : IPaginationResponse<T>
    {
        var limit = 250;
        string? cursor = null;
        TV response;

        var result = new List<T>();

        do
        {
            variables["after"] = cursor;
            variables["limit"] = limit;
            var request = new GraphQLRequest()
            {
                Query = query,
                Variables = variables
            };
            response = await ExecuteWithErrorHandling<TV>(request, cancellationToken);

            result.AddRange(response.Items.Nodes);
            cursor = response.Items.PageInfo.EndCursor;
        } while (response.Items.PageInfo.HasNextPage);

        return result;
    }

    public static string GenerateApiUrl(AuthenticationCredentialsProvider[] creds, string apiVersion) =>
        $"https://{creds.Get(CredsNames.StoreName).Value}.myshopify.com/admin/api/{apiVersion}/graphql.json";
}