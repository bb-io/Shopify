using Apps.Shopify.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Shopify.Services;

public class ContentServiceFactory(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
{
    public IContentService GetContentService(TranslatableResource contentType)
    {
        return contentType switch
        {
            TranslatableResource.COLLECTION => new CollectionService(invocationContext, fileManagementClient),
            TranslatableResource.METAFIELD => new MetafieldService(invocationContext, fileManagementClient),
            TranslatableResource.ARTICLE => new ArticleService(invocationContext, fileManagementClient),
            TranslatableResource.BLOG => new BlogService(invocationContext, fileManagementClient),
            TranslatableResource.PAGE => new PageService(invocationContext, fileManagementClient),
            TranslatableResource.ONLINE_STORE_THEME => new ThemeService(invocationContext, fileManagementClient),
            TranslatableResource.PRODUCT => new ProductService(invocationContext, fileManagementClient),
            _ => throw new Exception($"Unsupported content type '{contentType}' was passed in ContentServiceFactory")
        };
    }

    public List<IContentService> GetContentServices(IEnumerable<string> contentTypes)
    {
        var contentServices = new List<IContentService>();

        foreach (var contentType in contentTypes)
        {
            var enumType = Enum.Parse<TranslatableResource>(contentType);
            contentServices.Add(GetContentService(enumType));
        }

        return contentServices;
    }

    public IEnumerable<IPollingContentService> GetPollingContentServices(IEnumerable<string> contentTypes)
    {
        foreach (var type in contentTypes)
        {
            var service = GetContentService(Enum.Parse<TranslatableResource>(type));

            if (service is IPollingContentService pollingService)
                yield return pollingService;
        }
    }
}
