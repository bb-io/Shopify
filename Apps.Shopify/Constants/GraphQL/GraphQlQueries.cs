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

    public const string TranslatableResourceContent =
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
    
    public const string TranslatableResources =
        @"query ($resourceType: TranslatableResourceType!, $after: String, $limit: Int!) {
          translatableResources(first: $limit, after: $after, resourceType: $resourceType) {
              nodes {
                 resourceId
                 translatableContent {
                    key
                    value
                 }
               }
              pageInfo {
                 endCursor
                 hasNextPage
                 startCursor
              }
          }
        }";

    public const string TranslatableResourceTranslations =
        @"query ($resourceId: ID!, $locale: String!) {
          translatableResource(resourceId: $resourceId) {
               translations(locale: $locale) {
                  key
                  value
                }
          }
        }";

    public const string Events =
      @"query ($url: URL, $after: String, $limit: Int!) {
          webhookSubscriptions(first: $limit, after: $after, callbackUrl: $url) {
              nodes {
                 id
                 topic
                 callbackUrl
               }
              pageInfo {
                 endCursor
                 hasNextPage
                 startCursor
              }
          }
        }";
}