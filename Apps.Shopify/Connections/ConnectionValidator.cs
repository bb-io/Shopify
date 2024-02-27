using Apps.Shopify.Api;
using Apps.Shopify.Constants.GraphQL;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using GraphQL;

namespace Apps.Shopify.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var client = new ShopifyClient(authenticationCredentialsProviders.ToArray());

        var request = new GraphQLRequest()
        {
            Query = GraphQlQueries.Locales
        }; 
        await client.ExecuteWithErrorHandling(request, cancellationToken);

        return new()
        {
            IsValid = true
        };
    }
}