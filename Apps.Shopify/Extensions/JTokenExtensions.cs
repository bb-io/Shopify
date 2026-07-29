using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Extensions;

public static class JTokenExtensions
{
    public static string? GetJsonLabel(this JToken? token)
    {
        return token switch
        {
            JObject obj => obj.Properties().FirstOrDefault()?.Name,
            JArray array => GetJsonLabel(array.FirstOrDefault()),
            JValue { Value: not null } val => val.ToString(CultureInfo.InvariantCulture),
            _ => null
        };
    }
}