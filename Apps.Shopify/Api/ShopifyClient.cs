using Apps.Shopify.Constants;
using Apps.Shopify.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Api;

public class ShopifyClient : GraphQLHttpClient
{
    private const string ApiVersion = "2024-01";

    public ShopifyClient(AuthenticationCredentialsProvider[] creds) : base(
        $"https://{creds.Get(CredsNames.StoreName).Value}.myshopify.com/admin/api/{ApiVersion}/graphql.json",
        new NewtonsoftJsonSerializer())
    {
        var token = creds.Get(CredsNames.Token).Value;
        HttpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", token);
    }

    public async Task<T> ExecuteWithErrorHandling<T>(GraphQLRequest request, CancellationToken cancellationToken)
    {
        var response = await SendQueryAsync<T>(request, cancellationToken);

        if (response.Errors is not null && response.Errors.Any())
            throw new(string.Join(';', response.Errors.Select(x => x.Message)));

        return response.Data;
    }

    public async Task<GraphQLResponse<JObject>> ExecuteWithErrorHandling(GraphQLRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SendQueryAsync<JObject>(request, cancellationToken);

        if (response.Errors is not null && response.Errors.Any())
            throw new(string.Join(';', response.Errors.Select(x => x.Message)));

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
}