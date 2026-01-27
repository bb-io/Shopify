using System.Globalization;

namespace Apps.Shopify.Extensions;

public static class StringExtensions
{
    public static string GetShopifyItemId(this string adminApiId) => adminApiId.Split('/').Last();

    public static string ToTitle(this string input)
    {
        string titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        string cleaned = titleCase.Replace('-', ' ').Replace('_', ' ');
        return cleaned;
    }
}