using Apps.Shopify.Models.Entities.Content;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Services;

namespace Apps.Shopify.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<SearchContentResponse> ExecuteMany(
        this List<IContentService> contentServices,
        SearchContentRequest request)
    {
        var result = new List<ContentItemEntity>();

        foreach (var contentService in contentServices)
        {
            var response = await contentService.Search(request);
            var items = response.Items;
            result.AddRange(items);
        }

        return new(result);
    }
}
