using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Services.Models;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Concrete;

public partial class ProductService : IPrimaryLocaleContentService
{
    public async Task PreparePrimaryLocaleUpdate(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches)
    {
        var roots = context.Patches.Where(x => x.ResourceId.StartsWith("gid://shopify/Product/", StringComparison.Ordinal))
            .Select(x => x.ResourceId).Distinct().ToList();
        if (roots.Count == 0) throw PrimaryLocaleUpdateContext.Invalid("Product file must identify its original parent product");
        var products = new Dictionary<string, JObject>();
        foreach (var id in roots)
        {
            var data = await context.Query(PrimaryLocaleGraphQl.Product, new { id });
            products[id] = data["product"] as JObject ?? throw PrimaryLocaleUpdateContext.Invalid($"Missing product: {id}");
        }
        var options = products.SelectMany(p => p.Value["options"]!.Select(o => (ProductId: p.Key, Option: o))).ToList();
        foreach (var patch in context.Patches.Where(x =>
            x.ResourceId.StartsWith("gid://shopify/ProductOption/", StringComparison.Ordinal) ||
            x.ResourceId.StartsWith("gid://shopify/ProductOptionValue/", StringComparison.Ordinal)))
        {
            var ownerCount = options.Count(x => x.Option.Value<string>("id") == patch.ResourceId ||
                x.Option["optionValues"]!.Any(v => v.Value<string>("id") == patch.ResourceId));
            if (ownerCount != 1) throw PrimaryLocaleUpdateContext.Invalid($"Nested resource does not belong to exactly one exported product: {patch.ResourceId}");
        }
        var optionGroups = new Dictionary<string, List<IdentifiedContentRequest>>();
        foreach (var patch in patches)
        {
            if (products.ContainsKey(patch.ResourceId) || patch.ResourceId.StartsWith("gid://shopify/Metafield/", StringComparison.Ordinal)) continue;
            var owners = options.Where(x => x.Option.Value<string>("id") == patch.ResourceId ||
                x.Option["optionValues"]!.Any(v => v.Value<string>("id") == patch.ResourceId)).ToList();
            if (owners.Count != 1) throw PrimaryLocaleUpdateContext.Invalid($"Nested resource does not belong to an exported product: {patch.ResourceId}");
            var option = owners[0].Option;
            if (patch.Key != "name") { context.Unsupported(patch, "Only existing option/value names are writable"); continue; }
            if (option["linkedMetafield"]?.Type == JTokenType.Object ||
                option["optionValues"]!.Any(v => v.Value<string>("id") == patch.ResourceId && v["linkedMetafieldValue"]?.Type == JTokenType.String))
            { context.Unsupported(patch, "Linked metafield option names cannot be changed safely without altering references"); continue; }
            var optionId = option.Value<string>("id")!;
            if (!optionGroups.ContainsKey(optionId)) optionGroups[optionId] = [];
            optionGroups[optionId].Add(patch);
        }
        foreach (var product in products)
        {
            var fields = patches.Where(x => x.ResourceId == product.Key).ToList();
            var writable = new List<IdentifiedContentRequest>();
            var update = new JObject { ["id"] = product.Key };
            foreach (var patch in fields)
            {
                switch (patch.Key)
                {
                    case "title": update["title"] = patch.Value; break;
                    case "body_html": update["descriptionHtml"] = patch.Value; break;
                    case "meta_title": case "meta_description":
                        // SEOInput updates only supplied properties; never send omitted/null properties.
                        update["seo"] ??= new JObject();
                        update["seo"]![patch.Key == "meta_title" ? "title" : "description"] = patch.Value;
                        break;
                    default: context.Unsupported(patch, "Field is not allowlisted; handles and unrelated product properties are protected"); continue;
                }
                writable.Add(patch);
            }
            if (writable.Count > 0) context.Add(PrimaryLocaleGraphQl.ProductUpdate, new { product = update }, writable);
        }
        foreach (var group in optionGroups)
        {
            var owner = options.Single(x => x.Option.Value<string>("id") == group.Key);
            var optionInput = new JObject { ["id"] = group.Key };
            var name = group.Value.SingleOrDefault(x => x.ResourceId == group.Key);
            if (name != null) optionInput["name"] = name.Value;
            var variables = new JObject { ["productId"] = owner.ProductId, ["option"] = optionInput };
            var values = group.Value.Where(x => x.ResourceId != group.Key).Select(x => new JObject { ["id"] = x.ResourceId, ["name"] = x.Value }).ToList();
            if (values.Count > 250) throw PrimaryLocaleUpdateContext.Invalid("More than 250 option values in one update; split the file");
            if (values.Count > 0) variables["values"] = new JArray(values);
            context.Add(PrimaryLocaleGraphQl.OptionUpdate, variables, group.Value);
        }
        await MetafieldService.PrepareMetafields(context,
            patches.Where(x => x.ResourceId.StartsWith("gid://shopify/Metafield/", StringComparison.Ordinal)).ToList(), roots);
    }
}
