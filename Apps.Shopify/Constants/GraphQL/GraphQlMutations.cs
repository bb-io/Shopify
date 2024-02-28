namespace Apps.Shopify.Constants.GraphQL;

public static class GraphQlMutations
{
    public const string TranslationsRegister =
        @"mutation translationsRegister($resourceId: ID!, $translations: [TranslationInput!]!) {
                translationsRegister(resourceId: $resourceId, translations: $translations) {
                  userErrors {
                    message
                    field
                  }
                  translations {
                    key
                    value
                  }
                }
              }";
}