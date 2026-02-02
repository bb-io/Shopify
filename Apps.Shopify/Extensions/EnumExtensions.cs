using System.Globalization;

namespace Apps.Shopify.Extensions;

public static class EnumExtensions
{
    public static string ToTitle(this TranslatableResource input)
    {
        var text = input.ToString().Replace("_", " ").Replace("-", " ");
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }
}
