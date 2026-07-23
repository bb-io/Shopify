using Apps.Shopify.Constants;
using Apps.Shopify.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Apps.Shopify.Api;

public class ShopifyClient : GraphQLHttpClient
{
    public ShopifyClient(AuthenticationCredentialsProvider[] creds, string? endpoint = default) : base(
        endpoint ?? GenerateApiUrl(creds, ApiConstants.ApiVersion),
        new NewtonsoftJsonSerializer())
    {
        var token = ResolveAccessToken(creds);
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
            throw new PluginApplicationException(FormatHttpErrorMessage("GraphQL request", ex));
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
        GraphQLResponse<JObject> response;

        try
        {
            response = await SendQueryAsync<JObject>(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new PluginApplicationException(FormatHttpErrorMessage("GraphQL request", ex));
        }

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
        $"https://{GetRequiredCredentialValue(creds, CredsNames.StoreName)}.myshopify.com/admin/api/{apiVersion}/graphql.json";

    private static string ResolveAccessToken(AuthenticationCredentialsProvider[] creds)
    {
        var accessToken = GetCredentialValue(creds, CredsNames.Token);
        if (!string.IsNullOrWhiteSpace(accessToken))
            return accessToken;

        var clientId = GetRequiredCredentialValue(creds, CredsNames.ClientId);
        var clientSecret = GetRequiredCredentialValue(creds, CredsNames.ClientSecret);
        var storeName = GetRequiredCredentialValue(creds, CredsNames.StoreName);

        return RequestAccessTokenAsync(storeName, clientId, clientSecret).GetAwaiter().GetResult();
    }

    private static async Task<string> RequestAccessTokenAsync(string storeName, string clientId, string clientSecret)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://{storeName}.myshopify.com/admin/oauth/access_token");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        });

        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            throw new PluginApplicationException(FormatHttpErrorMessage("token request", ex));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new PluginApplicationException(
                $"Failed to retrieve Shopify access token: {(int)response.StatusCode} {response.ReasonPhrase}. {responseContent}");

        var tokenResponse = JObject.Parse(responseContent);
        var accessToken = tokenResponse.Value<string>("access_token");

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new PluginApplicationException("Shopify token response did not include an access token.");

        return accessToken;
    }

    private static string GetRequiredCredentialValue(AuthenticationCredentialsProvider[] creds, string keyName)
    {
        var value = GetCredentialValue(creds, keyName);
        if (string.IsNullOrWhiteSpace(value))
            throw new PluginApplicationException($"Missing required connection value: {keyName}");

        return value;
    }

    private static string? GetCredentialValue(AuthenticationCredentialsProvider[] creds, string keyName) =>
        creds.FirstOrDefault(x => x.KeyName == keyName)?.Value;

    private static string FormatHttpErrorMessage(string operation, HttpRequestException ex)
    {
        var statusCode = ex.StatusCode.HasValue
            ? $"{(int)ex.StatusCode.Value} {ex.StatusCode.Value}"
            : "unknown status";

        return $"HTTP error during {operation}: {statusCode}. {ex.Message}";
    }
}
