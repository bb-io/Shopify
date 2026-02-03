using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using System.Net;

namespace Apps.Shopify.Webhooks;

public class BaseWebhookList
{
    protected static Task<WebhookResponse<T>> HandleWebhookRequest<T>(WebhookRequest request) where T : class
    {
        var payload = request.Body.ToString();
        ArgumentException.ThrowIfNullOrEmpty(payload);

        var data = JsonConvert.DeserializeObject<T>(payload);

        return Task.FromResult<WebhookResponse<T>>(new()
        {
            Result = data,
            HttpResponseMessage = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK }
        });
    }
}
