using System.Text;
using System.Text.Json;

namespace Apps.Shopify.Services.Models;

/// <summary>Structural string-token patches. Offsets preserve comments, headers and all unrelated bytes.</summary>
public static class ThemeJsonPatch
{
    public record Location(string Key, string Value, int Start, int Length);

    public static List<Location> Locations(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var reader = new Utf8JsonReader(bytes, new JsonReaderOptions { CommentHandling = JsonCommentHandling.Skip });
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            throw PrimaryLocaleUpdateContext.Invalid("Theme locale asset must be a JSON object");
        var locations = new List<Location>();
        Read(ref reader, "", locations);
        if (reader.Read()) throw PrimaryLocaleUpdateContext.Invalid("Unexpected content after theme JSON object");
        return locations;
    }

    private static void Read(ref Utf8JsonReader reader, string path, List<Location> locations)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                var names = new HashSet<string>(StringComparer.Ordinal);
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName) throw PrimaryLocaleUpdateContext.Invalid("Invalid JSON property");
                    var key = reader.GetString()!;
                    if (!names.Add(key)) throw PrimaryLocaleUpdateContext.Invalid($"Duplicate JSON property: {path}.{key}");
                    if (!reader.Read()) throw PrimaryLocaleUpdateContext.Invalid("Incomplete JSON property");
                    Read(ref reader, path.Length == 0 ? key : path + "." + key, locations);
                }
                if (reader.TokenType != JsonTokenType.EndObject) throw PrimaryLocaleUpdateContext.Invalid("Incomplete JSON object");
                break;
            case JsonTokenType.StartArray:
                int index = 0;
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    Read(ref reader, path + "." + index++, locations);
                if (reader.TokenType != JsonTokenType.EndArray) throw PrimaryLocaleUpdateContext.Invalid("Incomplete JSON array");
                break;
            case JsonTokenType.String:
                locations.Add(new(path, reader.GetString()!, checked((int)reader.TokenStartIndex), checked((int)(reader.BytesConsumed - reader.TokenStartIndex))));
                break;
        }
    }

    public static string Apply(string original, IEnumerable<(Location Location, string Value)> patches)
    {
        var bytes = Encoding.UTF8.GetBytes(original);
        using var output = new MemoryStream();
        int position = 0;
        foreach (var patch in patches.OrderBy(x => x.Location.Start))
        {
            if (patch.Location.Start < position) throw PrimaryLocaleUpdateContext.Invalid("Conflicting theme locations");
            output.Write(bytes, position, patch.Location.Start - position);
            output.Write(JsonSerializer.SerializeToUtf8Bytes(patch.Value));
            position = patch.Location.Start + patch.Location.Length;
        }
        output.Write(bytes, position, bytes.Length - position);
        var result = Encoding.UTF8.GetString(output.ToArray());
        _ = Locations(result); // validate the complete merged asset before any write
        return result;
    }
}
