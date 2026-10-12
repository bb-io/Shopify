using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.PaymentGateway;

public class PaymentGatewayResponse(ContentItemEntity contentEntity)
{
    [Display("Payment gateway ID")]
    public string Id { get; set; } = contentEntity.ContentId;

    [Display("Payment gateway name")]
    public string Name { get; set; } = contentEntity.Name;
}