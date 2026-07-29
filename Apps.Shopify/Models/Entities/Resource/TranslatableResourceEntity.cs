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
}