using Apps.Shopify.Constants;
using Apps.Shopify.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Services;

public class ContentServiceFactory(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
{
    public IContentService GetContentService(string contentType)
    {
        string normalizedType = char.ToUpper(contentType[0]) + contentType.Substring(1).ToLower();
        return normalizedType switch
        {
            TranslatableResources.Collection => new CollectionService(invocationContext, fileManagementClient),
            TranslatableResources.Metafield => new MetafieldService(invocationContext, fileManagementClient),
            TranslatableResources.Article => new ArticleService(invocationContext, fileManagementClient),
            TranslatableResources.Blog => new BlogService(invocationContext, fileManagementClient),
            TranslatableResources.Page => new PageService(invocationContext, fileManagementClient),
            TranslatableResources.Theme => new ThemeService(invocationContext, fileManagementClient),
            TranslatableResources.Product => new ProductService(invocationContext, fileManagementClient),
            _ => throw new Exception($"Unsupported content type '{contentType}' was passed in ContentServiceFactory")
        };
    }

    public List<IContentService> GetContentServices(IEnumerable<string> contentTypes)
    {
        var contentServices = new List<IContentService>();

        foreach (var contentType in contentTypes)
            contentServices.Add(GetContentService(contentType));

        return contentServices;
    }

    public IEnumerable<IPollingContentService> GetPollingContentServices(IEnumerable<string> contentTypes)
    {
        foreach (var contentType in contentTypes)
        {
            var service = GetContentService(contentType);

            if (service is IPollingContentService pollingService)
                yield return pollingService;
        }
    }
}
