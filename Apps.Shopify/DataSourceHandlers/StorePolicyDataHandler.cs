using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class StorePolicyDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var service = new StorePolicyService(InvocationContext);
        
        var searchRequest = new SearchContentRequest { NameContains = context.SearchString };
        var searchResponse = await service.Search(searchRequest, firstPageOnly: true, cancellationToken);

        return searchResponse.Items.Select(x => new DataSourceItem(x.ContentId, x.Name)).ToList();
    }
}