namespace Apps.Shopify.Extensions;

public static class StringExtensions
{
    public static string GetShopifyItemId(this string adminApiId) => adminApiId.Split('/').Last();

    public static string GetFileName(this string contentFullId, string? marketFullId)
    {
        string contentId = contentFullId.GetShopifyItemId();
        
        string market = string.Empty;
        if (!string.IsNullOrWhiteSpace(marketFullId))
            market = $"_market{marketFullId.GetShopifyItemId()}";
        
        return $"{contentId}{market}.html";
    }
    
    public static string ToLinkGid(this string menuItemGid) => $"gid://shopify/Link/{menuItemGid.Split('/')[^1]}";
}