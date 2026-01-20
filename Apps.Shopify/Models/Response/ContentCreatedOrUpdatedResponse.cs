using Apps.Shopify.Models.Response.Content;

namespace Apps.Shopify.Models.Response;

public class ContentCreatedOrUpdatedResponse(List<ContentResponse> content)
{
    public List<ContentResponse> Content { get; set; } = content;
}