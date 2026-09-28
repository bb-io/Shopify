using System.Text.RegularExpressions;

namespace Apps.Shopify.Extensions;

public static class StringExtensions
{
    private static readonly Regex UnsafeFileNameCharsRegex = new(@"[^\w.\-]", RegexOptions.Compiled);
    
    public static string GetShopifyItemId(this string adminApiId) => adminApiId.Split('/').Last();

    public static string GetFileName(this string contentFullId, string? marketFullId)
    {
        string contentId = UnsafeFileNameCharsRegex.Replace(contentFullId.GetShopifyItemId(), "_");
        
        string market = string.Empty;
        if (!string.IsNullOrWhiteSpace(marketFullId))
            market = $"_market{marketFullId.GetShopifyItemId()}";
        
        return $"{contentId}{market}.html";
    }
    
    public static string ToLinkGid(this string menuItemGid) => $"gid://shopify/Link/{menuItemGid.Split('/')[^1]}";
}