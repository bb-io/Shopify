using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;

namespace Apps.Shopify.Services;

public interface IPollingContentService
{
    Task<ContentUpdatedResponse> PollUpdated(DateTime after, DateTime before, PollUpdatedContentRequest input);
}
