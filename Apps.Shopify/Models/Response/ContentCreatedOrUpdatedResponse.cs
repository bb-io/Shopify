using Apps.Shopify.Models.Entities.Content;

namespace Apps.Shopify.Models.Response;

public class ContentCreatedOrUpdatedResponse(List<ContentItemEntity> content)
{
    public List<ContentItemEntity> Content { get; set; } = content;
}