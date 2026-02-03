namespace Apps.Shopify.Helper;

public static class QueryHelper
{
    public static Dictionary<string, object> QueryToDictionary(string? query, Dictionary<string, object>? variables = null)
    {
        variables ??= [];

        if (!string.IsNullOrEmpty(query))
            variables["query"] = query;

        return variables;
    }
}
