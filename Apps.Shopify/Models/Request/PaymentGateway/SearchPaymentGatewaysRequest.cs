using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.PaymentGateway;

public class SearchPaymentGatewaysRequest
{
    [Display("Payment gateway name contains")]
    public string? NameContains { get; set; }
}