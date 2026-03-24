using Apps.Shopify.Models.Entities.Content;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Response.Content;

public record ContentUpdatedResponse(List<PollingContentItemEntity> Items) 
    : IMultiDownloadableContentOutput<PollingContentItemEntity>
{
    public List<PollingContentItemEntity> Items { get; set; } = Items;
}