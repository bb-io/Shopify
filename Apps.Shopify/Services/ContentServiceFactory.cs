using Apps.Shopify.Constants;
using Apps.Shopify.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.Services;

public class ContentServiceFactory(InvocationContext invocationContext)
{
    public IContentService GetContentService(string contentType)
    {
        string normalizedType = TranslatableResources.Normalize(contentType);
        return normalizedType switch
        {
            TranslatableResources.Collection => new CollectionService(invocationContext),
            TranslatableResources.Metafield => new MetafieldService(invocationContext),
            TranslatableResources.Article => new ArticleService(invocationContext),
            TranslatableResources.Blog => new BlogService(invocationContext),
            TranslatableResources.Page => new PageService(invocationContext),
            TranslatableResources.Theme => new ThemeService(invocationContext),
            TranslatableResources.Product => new ProductService(invocationContext),
            TranslatableResources.Menu => new MenuService(invocationContext),
            TranslatableResources.DeliveryMethodDefinition => new DeliveryMethodDefinitionService(invocationContext),
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
    
    public IEnumerable<IDigestPollingContentService> GetDigestPollingContentServices(IEnumerable<string> contentTypes)
    {
        foreach (var contentType in contentTypes)
        {
            var service = GetContentService(contentType);
            
            if (service is IDigestPollingContentService digestService)
                yield return digestService;
        }
    }
}
