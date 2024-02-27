namespace Apps.Shopify.Constants.GraphQL;

public static class GraphQlQueries
{
    public const string Locales =
        @"query {
          shopLocales {
            locale
            name
            primary
            published
          }
        }";

    public const string Products =
        @"query ($limit: Int!, $after: String, $query: String) {
            products (first: $limit, after: $after, query: $query){
              nodes {
                 id
                 title
                 handle
                 createdAt
                 publishedAt
                 description
                 onlineStoreUrl
                 productType
                 status
               }
              pageInfo {
                 endCursor
                 hasNextPage
                 startCursor
              }
            }
          }";
}