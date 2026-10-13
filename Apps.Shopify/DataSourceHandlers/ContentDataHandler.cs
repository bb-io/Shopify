using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ContentDataHandler : ShopifyInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _contentType;

    public ContentDataHandler(InvocationContext invocationContext, [ActionParameter] ContentTypeIdentifier contentType) 
        : base(invocationContext)
    {
        if (string.IsNullOrEmpty(contentType.ContentType))
            throw new PluginMisconfigurationException("Please specify content type first.");
        
        _contentType = contentType.ContentType;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var factory = new ContentServiceFactory(InvocationContext);
        var service = factory.GetContentService(_contentType);

        var searchRequest = new SearchContentRequest { NameContains = context.SearchString };
        var searchResult = await service.Search(searchRequest, firstPageOnly: true, cancellationToken);

        return searchResult.Items.Select(x => new DataSourceItem(x.ContentId, x.Name)).ToList();
    }
}
