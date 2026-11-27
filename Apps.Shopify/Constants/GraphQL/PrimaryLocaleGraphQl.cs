namespace Apps.Shopify.Constants.GraphQL;

public static class PrimaryLocaleGraphQl
{
    public const string Product = """
        query($id: ID!) { product(id: $id) {
          id seo { title description }
          options { id name linkedMetafield { namespace key } optionValues { id name linkedMetafieldValue } }
        } }
        """;
    public const string ProductUpdate = """
        mutation($product: ProductUpdateInput!) { productUpdate(product: $product) {
          product { id } userErrors { field message }
        } }
        """;
    public const string OptionUpdate = """
        mutation($productId: ID!, $option: OptionUpdateInput!, $values: [OptionValueUpdateInput!]) {
          productOptionUpdate(productId: $productId, option: $option, optionValuesToUpdate: $values, variantStrategy: LEAVE_AS_IS) {
            product { id } userErrors { field message }
          }
        }
        """;
    public const string Metafield = """
        query($id: ID!) { node(id: $id) { ... on Metafield {
          id owner { ... on Node { id } } namespace key type value compareDigest
        } } }
        """;
    public const string MetafieldsSet = """
        mutation($metafields: [MetafieldsSetInput!]!) { metafieldsSet(metafields: $metafields) {
          metafields { id } userErrors { field message }
        } }
        """;
    public const string Metaobject = """
        query($id: ID!) { metaobject(id: $id) {
          id fields { key type value } definition { fieldDefinitions { key type { name } } }
        } }
        """;
    public const string MetaobjectUpdate = """
        mutation($id: ID!, $metaobject: MetaobjectUpdateInput!) { metaobjectUpdate(id: $id, metaobject: $metaobject) {
          metaobject { id } userErrors { field message }
        } }
        """;
    public const string ThemeFiles = """
        query($id: ID!, $after: String) { theme(id: $id) {
          id files(first: 250, after: $after, filenames: ["locales/*.default.json"]) {
            nodes { filename checksumMd5 body { ... on OnlineStoreThemeFileBodyText { content } } }
            pageInfo { hasNextPage endCursor }
          }
        } }
        """;
    public const string ThemeFilesUpsert = """
        mutation($themeId: ID!, $files: [OnlineStoreThemeFilesUpsertFileInput!]!) {
          themeFilesUpsert(themeId: $themeId, files: $files) {
            upsertedThemeFiles { filename } job { id } userErrors { field message }
          }
        }
        """;
}
