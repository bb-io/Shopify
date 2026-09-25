using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Response.TranslatableResource;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers.TranslatableResourceBase;

public abstract class TranslatableResourceDataHandler(InvocationContext invocationContext)
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    protected abstract TranslatableResource ResourceType { get; }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var variables = new Dictionary<string, object> { ["resourceType"] = ResourceType };

        var response = await Client.PaginateOnce<TranslatableResourceEntity, TranslatableResourcePaginationResponse>(
            GraphQlQueries.TranslatableResources,
            variables,
            cancellationToken);

        return response
            .Where(x => x.MatchesSearch(context.SearchString))
            .Select(x => new DataSourceItem(x.ResourceId, x.GetDisplayName()))
            .ToList();
    }
}