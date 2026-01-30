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

    public const string ProductsWithMetafields =
        @"query ($limit: Int!, $after: String, $query: String, $metafieldKey: String!) {
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
                 metafield(key: $metafieldKey) {
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
        @"query ($outdated: Boolean, $resourceType: TranslatableResourceType!, $after: String, $limit: Int!, $locale: String!) {
          translatableResources(first: $limit, after: $after, resourceType: $resourceType) {
              nodes {
                 resourceId
                 translations(locale: $locale, outdated: $outdated) {
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
        @"query ($outdated: Boolean, $resourceIds: [ID!]!, $after: String, $limit: Int!, $locale: String!) {
          translatableResourcesByIds(first: $limit, after: $after, resourceIds: $resourceIds) {
              nodes {
                 resourceId
                 translations(locale: $locale, outdated: $outdated) {
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
        @"query ($outdated: Boolean, $resourceId: ID!, $locale: String!) {
          translatableResource(resourceId: $resourceId) {
               translations(locale: $locale, outdated: $outdated) {
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
    
    public const string TranslatableResourceTranslationKeys =
        @"query ($outdated: Boolean, $resourceId: ID!, $locale: String!) {
          translatableResource(resourceId: $resourceId) {
               translations(locale: $locale, outdated: $outdated) {
                  key
                }
                translatableContent {
                  key
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

    public const string MetafieldDefinitions =
        @"query ($limit: Int!, $after: String, $ownerType: MetafieldOwnerType!, $query: String) {
            metafieldDefinitions (first: $limit, after: $after, ownerType: $ownerType, query: $query) {
              nodes {
                 id
                 key
                 name
                 namespace
                 type {
                   name
                 }
               }
              pageInfo {
                 endCursor
                 hasNextPage
                 startCursor
              }
            }
          }";

    public const string MetafieldDefinition =
        @"query ($id: ID!) {
          metafieldDefinition(id: $id) {
            id
            name
            key
            namespace
            type{
              name
            }
          }
        }";

    public const string Collections =
        @"query ($limit: Int!, $after: String, $query: String) {
          collections(first: $limit, after: $after, query: $query) {
            nodes {
              id
              title
              description
              updatedAt
            }
            pageInfo {
              endCursor
              hasNextPage
              startCursor
            }
          }
        }";

    public const string Articles =
        @"query ($limit: Int!, $after: String, $query: String) {
          articles(first: $limit, after: $after, query: $query) {
            nodes {
              id
              title
              publishedAt
              createdAt
              updatedAt
              blog {
                id
              }
              author {
                name
              }
              handle
            }
            pageInfo {
              endCursor
              hasNextPage
              startCursor
            }
          }
        }";

    public const string Blogs =
        @"query ($limit: Int!, $after: String, $query: String) {
          blogs(first: $limit, after: $after, query: $query) {
            nodes {
              id
              title
              createdAt
              updatedAt
            }
            pageInfo {
              endCursor
              hasNextPage
              startCursor
            }
          }
        }";

    public const string Pages =
        @"query ($limit: Int!, $after: String, $query: String) {
          pages(first: $limit, after: $after, query: $query) {
            nodes {
              id
              title
              createdAt
              updatedAt
              publishedAt
              handle
            }
            pageInfo {
              endCursor
              hasNextPage
              startCursor
            }
          }
        }";

    public const string Themes =
        @"query ($limit: Int!, $after: String,  $roles: [ThemeRole!]) {
          themes(first: $limit, after: $after, roles: $roles) {
            nodes {
              id
              name
              role
              createdAt
              updatedAt
            }
            pageInfo {
              hasNextPage
              endCursor
              startCursor
            }
          }
        }";

    public const string TranslatableResourceIds =
        @"query ($resourceId: ID!) {
          translatableResource(resourceId: $resourceId) {
            resourceId
            translatableContent {
              key
            }
          }
        }";
}