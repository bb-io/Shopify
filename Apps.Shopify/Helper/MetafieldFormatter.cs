using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Helper;

public static class MetafieldFormatter
{
    public static string Prettify(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return string.Empty;

        JToken token;
        try
        {
            token = JToken.Parse(rawValue);
        }
        catch
        {
            return rawValue;
        }

        if (token is JObject obj && obj.ContainsKey("value") && obj.ContainsKey("unit"))
        {
            var val = obj["value"]?.ToString();
            var unit = obj["unit"]?.ToString().ToLower();
            return $"{val} {unit}";
        }

        if (token is JObject ratingObj && ratingObj.ContainsKey("value") && ratingObj.ContainsKey("scale_max"))
            return $"{ratingObj["value"]}/{ratingObj["scale_max"]}";

        if (token is JArray arr)
            return string.Join(", ", arr.Select(x => x.ToString()));

        return rawValue;
    }
}
