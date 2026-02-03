using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Helper;
using Apps.Shopify.Invocables;
using Apps.Shopify.Models.Entities.Blog;
using Apps.Shopify.Models.Response.Blog;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class BlogDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string? query = new QueryBuilder()
            .AddContains("title", context.SearchString)
            .Build();

        var response = await Client.Paginate<BlogEntity, BlogsPaginationResponse>(
            GraphQlQueries.Blogs,
            QueryHelper.QueryToDictionary(query),
            cancellationToken
        );

        var items = response.Select(x => new DataSourceItem(x.Id, x.Title)).ToList();
        return items;
    }
}