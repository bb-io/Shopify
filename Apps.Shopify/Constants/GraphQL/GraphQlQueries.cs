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
    
    public const string TranslatableResourcesWithTranslations =
        @"query ($resourceType: TranslatableResourceType!, $after: String, $limit: Int!, $locale: String!) {
          translatableResources(first: $limit, after: $after, resourceType: $resourceType) {
              nodes {
                 resourceId
                 translations(locale: $locale) {
                    key
                    value
                 }
                 translatableContent {
                    key
                    value
                    digest
                 }
               }
              pageInfo {
                 endCursor
                 hasNextPage
                 startCursor
              }
          }
        }";   
    
    public const string TranslatableResourcesByIds =
        @"query ($resourceIds: [ID!]!, $after: String, $limit: Int!, $locale: String!) {
          translatableResourcesByIds(first: $limit, after: $after, resourceIds: $resourceIds) {
              nodes {
                 resourceId
                 translations(locale: $locale) {
                    key
                    value
                 }
                 translatableContent {
                    key
                    value
                    digest
                 }
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

    public const string TranslatableResourceTranslations =
        @"query ($resourceId: ID!, $locale: String!) {
          translatableResource(resourceId: $resourceId) {
               translations(locale: $locale) {
                  key
                  value
                }
                translatableContent {
                  key
                  value
                  digest
                  locale
                  type
                }
          }
        }";

    public const string Events =
        @"query ($after: String, $limit: Int!) {
          webhookSubscriptions(first: $limit, after: $after) {
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
    
    public const string ProductMetaFields =
        @"query ($resourceId: ID!, $after: String, $limit: Int!) {
          metafields(first: $limit, after: $after, owner: $resourceId) {
            nodes {
              id
              namespace
              key
              value
              compareDigest
            },
            pageInfo {
               endCursor
               hasNextPage
               startCursor
            }
          }
        }";    
    
    public const string Product =
        @"query ($resourceId: ID!, $locale: String!) {
          product(id: $resourceId) {
            id
            title
            handle
            options{
             id            
             name            
             optionValues {
               id
               name
               translations(locale: $locale){
                key
                value
               }  
             }            
             translations(locale: $locale){
                key
                value
             }            
            }
            translations(locale: $locale){
                key
                value
             }  
          }
        }";
}