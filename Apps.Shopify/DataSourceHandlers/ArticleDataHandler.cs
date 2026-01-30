using Apps.Shopify.Invocables;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Entities.Article;
using Apps.Shopify.Models.Response.Article;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Shopify.DataSourceHandlers;

public class ArticleDataHandler(InvocationContext invocationContext) 
    : ShopifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var response = await Client.Paginate<ArticleEntity, ArticlesPaginationResponse>(
            GraphQlQueries.Articles, 
            [],
            cancellationToken
        );

        if (!string.IsNullOrEmpty(context.SearchString))
        {
            response = response.Where(x =>
                x.Title.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var items = response.Select(x => new DataSourceItem(x.Id, x.Title)).ToList();
        return items;
    }
}