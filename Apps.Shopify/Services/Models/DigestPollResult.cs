using Apps.Shopify.Models.Entities.Content;

namespace Apps.Shopify.Services.Models;

public record DigestPollResult(List<PollingContentItemEntity> Items, Dictionary<string, string> Digests);