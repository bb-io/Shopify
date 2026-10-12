using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Shopify.Models.Identifiers;

public class PaymentGatewayIdentifier
{
    [Display("Payment gateway ID"), DataSource(typeof(PaymentGatewayDataHandler))]
    public string PaymentGatewayId { get; set; } = string.Empty;
}