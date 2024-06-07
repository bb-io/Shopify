using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apps.Shopify.Constants;

public static class JsonConfig
{
    public static JsonSerializerSettings RestJsonSettings => new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };
}