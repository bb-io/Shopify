using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.PaymentGateway;

public record SearchPaymentGatewaysResponse([property: Display("Payment gateways")] List<PaymentGatewayResponse> Gateways);