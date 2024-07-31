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

    public const string SubscribeEvent =
        @"mutation webhookSubscriptionCreate($topic: WebhookSubscriptionTopic!, $webhookSubscription: WebhookSubscriptionInput!) {
          webhookSubscriptionCreate(topic: $topic, webhookSubscription: $webhookSubscription) {
            userErrors {
              field
              message
            }
          }
        }";

    public const string UnsubscribeEvent =
        @"mutation webhookSubscriptionDelete($id: ID!) {
          webhookSubscriptionDelete(id: $id) {
            deletedWebhookSubscriptionId
            userErrors {
              field
              message
            }
          }
        }";

    public const string MetafieldsSet =
        @"mutation MetafieldsSet($metafields: [MetafieldsSetInput!]!) {
          metafieldsSet(metafields: $metafields) {
            metafields {
              key
              value
            }
            userErrors {
              field
              message
              code
            }
          }
        }";
}