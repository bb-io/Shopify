namespace Apps.Shopify.Extensions;

public static class StringExtensions
{
    public static string GetShopifyItemId(this string adminApiId) => adminApiId.Split('/').Last();
}