using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Extensions;

public static class NewtonsoftExtensions
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
    
    public static IEnumerable<JToken> ChildNodes(this JToken node)
    {
        return node["children"] as JArray ?? Enumerable.Empty<JToken>();
    }
    
    public static bool IsText(this JObject node) => node.Value<string>("type") == "text";

    public static bool HasSameFormat(this JObject a, JObject b)
    {
        return a.Value<bool?>("bold") == b.Value<bool?>("bold") && a.Value<bool?>("italic") == b.Value<bool?>("italic");
    }
}