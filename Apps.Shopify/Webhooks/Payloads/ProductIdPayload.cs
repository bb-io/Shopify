using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Webhooks.Payloads;

public class ProductIdPayload
{
    [Display("Product ID")]
    public string Id { get; set; }
}