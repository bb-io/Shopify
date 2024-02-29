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

    public const string ProductContent =
        @"query ($resourceId: ID!) {
          translatableResource(resourceId: $resourceId) {
            translatableContent {
              key
              value
              digest
              locale
              type
            }
          }
        }";

    public const string ProductTranslationContent =
        @"query ($resourceId: ID!, $locale: String!) {
          translatableResource(resourceId: $resourceId) {
               translations(locale: $locale) {
                  key
                  value
                }
          }
        }";
}