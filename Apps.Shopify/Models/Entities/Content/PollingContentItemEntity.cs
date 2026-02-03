using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Content;

public class PollingContentItemEntity(string contentId, string contentType, string name, DateTime updatedAt) 
    : ContentItemEntity(contentId, contentType, name)
{
    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; } = updatedAt;
}
