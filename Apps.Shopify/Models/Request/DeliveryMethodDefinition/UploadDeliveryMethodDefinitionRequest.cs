using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.DeliveryMethodDefinition;

public class UploadDeliveryMethodDefinitionRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Delivery method definition ID"), DataSource(typeof(DeliveryMethodDefinitionDataHandler))]
    public string? DeliveryMethodDefinitionId { get; set; }
}