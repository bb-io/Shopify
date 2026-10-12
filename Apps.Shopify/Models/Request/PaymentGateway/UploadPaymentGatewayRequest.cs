using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.PaymentGateway;

public class UploadPaymentGatewayRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;
    
    [Display("Payment gateway ID"), DataSource(typeof(PaymentGatewayDataHandler))]
    public string? PaymentGatewayId { get; set; }
}