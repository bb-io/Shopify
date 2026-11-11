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
        if (Translations?.Any() != true)
            return TranslatableContent;

        // Translations come without a digest. Take it from the source so stale uploads can be detected
        var sourceDigests = TranslatableContent.DistinctBy(x => x.Key).ToDictionary(x => x.Key, x => x.Digest);
        return Translations.Select(x => new ContentEntity
        {
            Key = x.Key,
            Value = x.Value,
            Digest = sourceDigests.GetValueOrDefault(x.Key) ?? string.Empty
        });
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

    public string? GetName()
    {
        string[] nameKeys = ["title", "name", "label"];

        return nameKeys
            .Select(key => TranslatableContent.FirstOrDefault(t => t.Key == key)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    public string GetDisplayName() => GetName() ?? GetDisplayNameFromId() ?? ToString();

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

    private string? GetDisplayNameFromId()
    {
        Dictionary<string, string> displayNameFormats = new()
        {
            ["OnlineStoreThemeLocaleContent"] = "Default theme content (theme {0})",
            ["OnlineStoreThemeSettingsDataSections"] = "Theme sections (theme {0})",
            ["PackingSlipTemplate"] = "Packing slip template {0}"
        };
        
        if (!Uri.TryCreate(ResourceId, UriKind.Absolute, out var uri) || uri.Segments.Length < 3)
            return null;

        string id = HttpUtility.UrlDecode(uri.Segments[^1]);
        string type = uri.Segments[^2].TrimEnd('/');

        string? themeId = HttpUtility.ParseQueryString(uri.Query)["theme_id"];
        if (themeId is not null)
            return $"{id} (theme {themeId})";

        return displayNameFormats.TryGetValue(type, out var format) ? string.Format(format, id) : null;
    }
}