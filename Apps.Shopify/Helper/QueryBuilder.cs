namespace Apps.Shopify.Helper;

public class QueryBuilder
{
    private readonly List<string> _parts = [];
    private const string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";

    public QueryBuilder Add(string field, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _parts.Add($"{field}:{value}");

        return this;
    }

    public QueryBuilder AddEquals(string field, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _parts.Add($"{field}:{value}");

        return this;
    }

    public QueryBuilder AddContains(string field, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _parts.Add($"{field}:*{value}*");

        return this;
    }

    public QueryBuilder AddDateRange(string field, DateTime? after, DateTime? before)
    {
        if (after.HasValue)
            _parts.Add($"{field}:>'{after.Value.ToString(DateFormat)}'");

        if (before.HasValue)
            _parts.Add($"{field}:<'{before.Value.ToString(DateFormat)}'");

        return this;
    }

    public QueryBuilder AddOr(string field, IEnumerable<string>? values)
    {
        if (values == null || !values.Any()) 
            return this;

        var conditions = values.Select(v => $"{field}:{v}").ToList();
        if (conditions.Count == 1)
            _parts.Add(conditions.First());
        else
            _parts.Add($"({string.Join(" OR ", conditions)})");

        return this;
    }

    public string? Build()
    {
        return _parts.Count > 0 ? string.Join(" AND ", _parts) : null;
    }
}
