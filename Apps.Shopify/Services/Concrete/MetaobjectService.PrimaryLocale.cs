using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Services.Models;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Concrete;

public partial class MetaobjectService : IPrimaryLocaleContentService
{
    public async Task PreparePrimaryLocaleUpdate(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches)
    {
        foreach (var group in patches.GroupBy(x => x.ResourceId))
        {
            if (!group.Key.StartsWith("gid://shopify/Metaobject/", StringComparison.Ordinal))
                throw PrimaryLocaleUpdateContext.Invalid($"Expected original Metaobject GID: {group.Key}");
            var data = await context.Query(PrimaryLocaleGraphQl.Metaobject, new { id = group.Key });
            var obj = data["metaobject"] as JObject ?? throw PrimaryLocaleUpdateContext.Invalid($"Metaobject no longer exists: {group.Key}");
            var fields = new JArray();
            var writable = new List<IdentifiedContentRequest>();
            foreach (var patch in group)
            {
                var definitions = obj["definition"]!["fieldDefinitions"]!.Where(x => x.Value<string>("key") == patch.Key).ToList();
                var current = obj["fields"]!.Where(x => x.Value<string>("key") == patch.Key).ToList();
                if (definitions.Count != 1 || current.Count != 1)
                    throw PrimaryLocaleUpdateContext.Invalid($"Missing or ambiguous metaobject field definition: {group.Key}, {patch.Key}");
                var type = definitions[0]["type"]!.Value<string>("name")!;
                if (!PrimaryLocaleValue.TryConvert(type, patch.Value, out var value))
                { context.Unsupported(patch, $"Unsupported field type '{type}'; references and structured components are preserved"); continue; }
                fields.Add(new JObject { ["key"] = patch.Key, ["value"] = value });
                writable.Add(patch);
            }
            if (fields.Count > 250) throw PrimaryLocaleUpdateContext.Invalid("More than 250 metaobject fields; split the file");
            // MetaobjectUpdateInput.fields maps by key and leaves omitted fields unchanged.
            if (fields.Count > 0) context.Add(PrimaryLocaleGraphQl.MetaobjectUpdate, new { id = group.Key, metaobject = new { fields } }, writable);
        }
    }
}
