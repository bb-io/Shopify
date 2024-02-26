using Apps.Shopify.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Shopify.Connections;

public class ConnectionValidator: IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var creds = authenticationCredentialsProviders.ToArray();
        var client = new ShopifyClient(creds);

        var request = new ShopifyRequest("customers.json", Method.Get, creds);
        await client.ExecuteWithErrorHandling(request);
        
        return new()
        {
            IsValid = true
        };
    }
}