using Apps.Shopify.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Models.Entities.Resource;

public class TranslatableResourceEntity
{
    private static readonly string[] DisplayNameKeys = ["title", "name"];
    
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
        return DisplayNameKeys
                   .Select(key => TranslatableContent.FirstOrDefault(t => t.Key == key)?.Value)
                   .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
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
}