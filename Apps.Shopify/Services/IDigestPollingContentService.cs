using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Services;

public interface IDigestPollingContentService
{
    Task<DigestPollResult> PollUpdated(IReadOnlyDictionary<string, string> knownDigests, PollUpdatedContentRequest input);
}