using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Shopify;

// This is a class only for development environment, cannot be pushed to production
public class ShopifyLogger
{
    private const string LogUrl = "https://webhook.site/5dd13e25-546f-4d11-b1dd-cdb06509e03e";
    
    public static async Task LogAsync<T>(T obj) where T : class
    {
        var restClient = new RestClient(LogUrl);
        var request = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        
        await restClient.ExecuteAsync(request);
    }

    public static Task LogAsync(Exception exception)
    {
        return LogAsync(new
        {
            Exception = exception.Message,
            StackTrace = exception.StackTrace,
            ExceptionType = exception.GetType().Name
        });
    } 
}