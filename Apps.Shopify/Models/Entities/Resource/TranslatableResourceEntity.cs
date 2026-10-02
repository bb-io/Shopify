using System.Web;
using Apps.Shopify.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Models.Entities.Resource;

public class TranslatableResourceEntity
{
    public string ResourceId { get; set; }

    public IEnumerable<ContentEntity> TranslatableContent { get; set; }

    public IEnumerable<ContentEntity> Translations { get; set; }

    public IEnumerable<ContentEntity> GetTranslatableContent()
    {
        return Translations.Any()
            ? Translations
            : TranslatableContent;
    }

    public override string ToString()
    {
        var value = TranslatableContent?.FirstOrDefault()?.Value;
        if (string.IsNullOrWhiteSpace(value))
            return ResourceId;

        if (value.StartsWith('{') || value.StartsWith('['))
        {
            try
            {
                value = JToken.Parse(value).GetJsonLabel() ?? value;
            }
            catch (JsonReaderException)
            {
                // Not valid JSON - keep the raw value
            }
        }

        var singleLine = string.Join(
            ' ',
            value.Split(['\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        return singleLine.Length <= 50 ? singleLine : singleLine[..50] + "...";
    }

    public string GetDisplayName()
    {
        string[] displayNameKeys = ["title", "name", "label"];
        
        return displayNameKeys
                   .Select(key => TranslatableContent.FirstOrDefault(t => t.Key == key)?.Value)
                   .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
               ?? GetThemeFileName()
               ?? ToString();
    }

    public bool MatchesSearch(string? searchString)
    {
        return string.IsNullOrEmpty(searchString) ||
               GetDisplayName().Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
               ResourceId.Contains(searchString, StringComparison.OrdinalIgnoreCase);
    }

    public string GetContentDigest()
    {
        return string.Join('|', TranslatableContent.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Digest}"));
    }

    public bool HasContent()
    {
        return TranslatableContent.Any(x => !string.IsNullOrWhiteSpace(x.Value));
    }

    // Examples:
    // gid://shopify/OnlineStoreThemeJsonTemplate/index?theme_id=162863874332   -> "index (theme 162863874332)"
    // gid://shopify/OnlineStoreThemeLocaleContent/208521298258                 -> "Default theme content (theme 208521298258)"
    private string? GetThemeFileName()
    {
        const string themeLocaleContentType = "OnlineStoreThemeLocaleContent";
        
        if (!Uri.TryCreate(ResourceId, UriKind.Absolute, out var uri) || uri.Segments.Length < 3)
            return null;

        string id = uri.Segments[^1];
        string type = uri.Segments[^2].TrimEnd('/');

        string? themeId = HttpUtility.ParseQueryString(uri.Query)["theme_id"];
        if (themeId is not null)
            return $"{id} (theme {themeId})";

        return type == themeLocaleContentType ? $"Default theme content (theme {id})" : null;
    }
}