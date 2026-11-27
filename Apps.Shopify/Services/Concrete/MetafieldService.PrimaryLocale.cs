using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Services.Models;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Concrete;

public partial class MetafieldService : IPrimaryLocaleContentService
{
    public Task PreparePrimaryLocaleUpdate(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches) =>
        PrepareMetafields(context, patches);

    public static async Task PrepareMetafields(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches,
        IReadOnlyCollection<string>? productOwners = null)
    {
        var prepared = new List<(JObject Input, IdentifiedContentRequest Patch)>();
        var fieldsById = new Dictionary<string, JObject>();
        foreach (var id in context.Patches.Where(x => x.ResourceId.StartsWith("gid://shopify/Metafield/", StringComparison.Ordinal))
            .Select(x => x.ResourceId).Distinct())
        {
            var data = await context.Query(PrimaryLocaleGraphQl.Metafield, new { id });
            var field = data["node"] as JObject;
            if (field?.Value<string>("id") != id || field["owner"]?.Value<string>("id") == null)
                throw PrimaryLocaleUpdateContext.Invalid($"Metafield no longer exists or is inaccessible: {id}");
            var ownerId = field["owner"]!.Value<string>("id")!;
            if (productOwners != null && !productOwners.Contains(ownerId))
                throw PrimaryLocaleUpdateContext.Invalid($"Metafield {id} does not belong to an exported product");
            fieldsById[id] = field;
        }
        foreach (var patch in patches)
        {
            var field = fieldsById[patch.ResourceId];
            var ownerId = field["owner"]!.Value<string>("id")!;
            if (patch.Key != "value" || !PrimaryLocaleValue.TryConvert(field.Value<string>("type")!, patch.Value, out var value))
            { context.Unsupported(patch, "Only complete text/rich-text values are supported; structured components, lists and references are preserved"); continue; }
            if (field["compareDigest"]?.Type != JTokenType.String)
                throw PrimaryLocaleUpdateContext.Invalid($"Shopify did not provide metafield compareDigest: {patch.ResourceId}");
            prepared.Add((new JObject
            {
                ["ownerId"] = ownerId, ["namespace"] = field["namespace"]!.DeepClone(), ["key"] = field["key"]!.DeepClone(),
                ["type"] = field["type"]!.DeepClone(), ["value"] = value, ["compareDigest"] = field["compareDigest"]!.DeepClone()
            }, patch));
        }
        foreach (var chunk in prepared.Chunk(25))
        {
            if (System.Text.Encoding.UTF8.GetByteCount(new JArray(chunk.Select(x => x.Input)).ToString()) > 9_500_000)
                throw PrimaryLocaleUpdateContext.Invalid("Metafield batch exceeds the Shopify payload limit; split the file");
            context.Add(PrimaryLocaleGraphQl.MetafieldsSet, new { metafields = chunk.Select(x => x.Input).ToArray() }, chunk.Select(x => x.Patch), async () =>
            {
                foreach (var item in chunk)
                {
                    var latest = await context.Query(PrimaryLocaleGraphQl.Metafield, new { id = item.Patch.ResourceId });
                    var field = latest["node"];
                    if (field?.Value<string>("id") != item.Patch.ResourceId || field.Value<string>("compareDigest") != item.Input.Value<string>("compareDigest"))
                        throw PrimaryLocaleUpdateContext.Invalid($"Metafield changed or was removed before write: {item.Patch.ResourceId}");
                }
            });
        }
    }
}
