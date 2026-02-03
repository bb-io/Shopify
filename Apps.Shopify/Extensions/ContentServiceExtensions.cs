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
        var searchTasks = contentServices.Select(service => service.Search(request));
        var results = await Task.WhenAll(searchTasks);

        var allItems = results.SelectMany(x => x.Items).ToList();
        return new(allItems);
    }
}
