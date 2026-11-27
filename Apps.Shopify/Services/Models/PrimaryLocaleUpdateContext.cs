using Apps.Shopify.Constants;
using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.HtmlConversion;
using Apps.Shopify.HtmlConversion.Models;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Models.Response.Content;
using Blackbird.Applications.Sdk.Common.Exceptions;
using GraphQL;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Models;

/// <summary>Shared preflight and execution for capability implementations in existing services.</summary>
public class PrimaryLocaleUpdateContext(UpdatePrimaryLocaleContentRequest input, Func<GraphQLRequest, Task<JObject>> execute)
{
    public const string ApiVersion = "2026-10";
    public UpdatePrimaryLocaleContentResponse Result { get; } = new() { DryRun = input.DryRun ?? true };
    public List<IdentifiedContentRequest> Patches { get; private set; } = [];
    private readonly List<(GraphQLRequest Request, List<IdentifiedContentRequest> Fields, Func<Task>? Guard, Func<Task<bool>>? Confirm)> plans = [];
    private readonly Dictionary<string, JObject> sources = new(StringComparer.Ordinal);
    public Task<JObject> Query(string query, object variables) => execute(new GraphQLRequest { Query = query, Variables = variables });
    public static PluginMisconfigurationException Invalid(string message) => new(message);

    public async Task<UpdatePrimaryLocaleContentResponse> Run(string html, string contentType, IPrimaryLocaleContentService service)
    {
        Patches = Parse(html, contentType);
        var locales = await Query(GraphQlQueries.Locales, new { });
        var primary = locales["shopLocales"]!.Where(x => x.Value<bool>("primary")).ToList();
        if (primary.Count != 1) throw Invalid("Shopify must return exactly one primary locale");
        Result.PrimaryLocale = primary[0].Value<string>("locale")!;
        if (!string.IsNullOrWhiteSpace(input.ExpectedPrimaryLocale) &&
            !string.Equals(input.ExpectedPrimaryLocale, Result.PrimaryLocale, StringComparison.OrdinalIgnoreCase))
            throw Invalid($"Expected primary locale '{input.ExpectedPrimaryLocale}', connected store uses '{Result.PrimaryLocale}'");

        // Fetch all resources, even those whose translated values are empty.
        foreach (var id in Patches.Select(x => x.ResourceId).Distinct())
            sources[id] = await ReadSource(id);
        var changed = new List<IdentifiedContentRequest>();
        foreach (var patch in Patches)
        {
            var source = FindSource(sources[patch.ResourceId], patch);
            if (PrimaryLocaleValue.IsEmpty(patch.Value)) { Result.SkippedFields.Add(Field(patch, "Empty translated value")); continue; }
            if (patch.Value == source.Value<string>("value")) { Result.UnchangedFields.Add(Field(patch, "Already matches source")); continue; }
            ValidateDigest(patch, source);
            changed.Add(patch);
        }
        await service.PreparePrimaryLocaleUpdate(this, changed);
        Result.PreparedFields = plans.SelectMany(x => x.Fields).Select(x => Field(x)).ToList();
        Result.PreparedResources = Result.PreparedFields.Select(x => x.ResourceId).Distinct().Count();
        if (Result.DryRun) return Result;
        Result.Warnings.Add("Preflight validation is not atomic for products, options, metaobjects or themes. Concurrent edits can still race mutations without native compare-and-set.");
        // Recheck every planned resource before the first write.
        foreach (var group in plans.SelectMany(x => x.Fields).GroupBy(x => x.ResourceId))
        {
            var latest = await ReadSource(group.Key);
            foreach (var patch in group)
                if (!JToken.DeepEquals(FindSource(sources[group.Key], patch), FindSource(latest, patch)))
                    throw Invalid($"Source changed during preflight: {patch.ResourceId}, {patch.Key}");
        }
        foreach (var plan in plans)
            if (plan.Guard != null) await plan.Guard();
        foreach (var plan in plans)
        {
            if (plan.Guard != null) await plan.Guard();
            JObject response;
            try { response = await execute(plan.Request); }
            catch (PluginApplicationException ex) when (plan.Request.Query == PrimaryLocaleGraphQl.ThemeFilesUpsert)
            { throw new PluginApplicationException($"Theme write unavailable: verify write_themes and Shopify's theme-file exemption. No alternative write mechanism was attempted. Shopify: {ex.Message}"); }
            var payload = response.Properties().Single().Value;
            var errors = payload["userErrors"] as JArray ?? throw Invalid("Shopify mutation returned no userErrors payload");
            if (errors.Count > 0)
                throw new PluginApplicationException($"Shopify rejected primary locale update: {errors}. Earlier successful mutations cannot be rolled back.");
            var jobId = payload["job"]?.Value<string>("id");
            if (jobId != null)
            {
                var job = await Query("query($id: ID!) { job(id: $id) { id done } }", new { id = jobId });
                if (job["job"]?.Value<bool?>("done") != true)
                {
                    Result.PendingJobs.Add(jobId);
                    continue;
                }
            }
            if (plan.Confirm != null && !await plan.Confirm())
                throw new PluginApplicationException("Shopify did not confirm the expected asset content after the theme write. No theme fields were counted as completed.");
            Result.UpdatedFields.AddRange(plan.Fields.Select(x => Field(x)));
        }
        Result.UpdatedResourceIds = Result.UpdatedFields.Select(x => x.ResourceId).Distinct().ToList();
        return Result;
    }

    public static List<IdentifiedContentRequest> Parse(string html, string contentType)
    {
        var doc = new HtmlDocument(); doc.LoadHtml(html);
        if (doc.DocumentNode.SelectNodes($"//meta[@name='{HtmlMetadataConstants.BlackbirdMarketId}']")?
            .Any(x => !string.IsNullOrWhiteSpace(x.GetAttributeValue("content", ""))) == true)
            throw Invalid("Market-specific files cannot update global source content");
        var metadataType = ShopifyHtmlConverter.ExtractContentTypeFromHtml(html);
        if (metadataType != null && TranslatableResources.Normalize(metadataType) != contentType)
            throw Invalid("Content type does not match file metadata");
        IEnumerable<IdentifiedContentRequest> items;
        if (contentType == TranslatableResources.Product)
        {
            var dto = ShopifyHtmlConverter.ProductToJson(html, "");
            items = dto.ProductContentEntities.Concat(dto.MetafieldsContentEntities ?? [])
                .Concat(dto.OptionsContentEntities ?? []).Concat(dto.OptionValuesContentEntities ?? []);
        }
        else items = ShopifyHtmlConverter.ToJson(html, "", new ShopifyMetadata());
        var patches = items.ToList();
        // Detect nodes a nested product converter could otherwise silently omit.
        if (doc.DocumentNode.Descendants().Count(x => x.Attributes["key"] != null) != patches.Count)
            throw Invalid("Unrecognized or ambiguous content structure in file");
        if (patches.Count == 0) throw Invalid("The file contains no patches");
        var allowedTypes = contentType switch
        {
            TranslatableResources.Product => new[] { "Product", "Metafield", "ProductOption", "ProductOptionValue" },
            TranslatableResources.Metafield => new[] { "Metafield" },
            TranslatableResources.Metaobject => new[] { "Metaobject" },
            TranslatableResources.Theme => new[] { "OnlineStoreTheme" },
            _ => throw Invalid($"Unsupported primary locale content type: {contentType}")
        };
        foreach (var patch in patches)
            if (!Uri.TryCreate(patch.ResourceId, UriKind.Absolute, out var uri) || uri.Scheme != "gid" || uri.Host != "shopify" ||
                uri.Segments.Length != 3 || !allowedTypes.Contains(uri.Segments[1].TrimEnd('/')) ||
                !long.TryParse(uri.Segments[2], out var numericId) || numericId <= 0 || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment) ||
                string.IsNullOrWhiteSpace(patch.Key) || !string.IsNullOrWhiteSpace(patch.MarketId))
                throw Invalid($"Missing or invalid original resource ID/key: '{patch.ResourceId}', '{patch.Key}'");
        foreach (var group in patches.GroupBy(x => (x.ResourceId, x.Key)))
            if (group.Select(x => (x.Value, x.TranslatableContentDigest)).Distinct().Count() > 1)
                throw Invalid($"Conflicting duplicate patches: {group.Key.ResourceId}, {group.Key.Key}");
        return patches.DistinctBy(x => (x.ResourceId, x.Key)).ToList();
    }

    private async Task<JObject> ReadSource(string id)
    {
        var data = await Query(GraphQlQueries.TranslatableResourceContent, new { resourceId = id });
        return data["translatableResource"] as JObject ?? throw Invalid($"Resource does not exist or is inaccessible: {id}");
    }
    private static JToken FindSource(JObject resource, IdentifiedContentRequest patch)
    {
        var matches = resource["translatableContent"]!.Where(x => x.Value<string>("key") == patch.Key).ToList();
        if (matches.Count != 1) throw Invalid($"Missing or ambiguous source key: {patch.ResourceId}, {patch.Key}");
        return matches[0];
    }
    private void ValidateDigest(IdentifiedContentRequest patch, JToken source)
    {
        if (string.IsNullOrWhiteSpace(patch.TranslatableContentDigest) || string.IsNullOrWhiteSpace(source.Value<string>("digest")))
        {
            var message = $"Missing source digest: {patch.ResourceId}, {patch.Key}. Some option exports omit digests; disable validation explicitly only after review.";
            if (input.ValidateSourceDigest ?? true) throw Invalid(message);
            Result.Warnings.Add(message);
        }
        else if ((input.ValidateSourceDigest ?? true) && patch.TranslatableContentDigest != source.Value<string>("digest"))
            throw Invalid($"Stale source digest: {patch.ResourceId}, {patch.Key}. Download again before updating.");
    }
    public void Unsupported(IdentifiedContentRequest patch, string reason)
    {
        if (!(input.SkipUnsupportedFields ?? false)) throw Invalid($"Unsupported field {patch.ResourceId}, {patch.Key}: {reason}");
        Result.SkippedFields.Add(Field(patch, reason));
    }
    public void Add(string query, object variables, IEnumerable<IdentifiedContentRequest> fields, Func<Task>? guard = null, Func<Task<bool>>? confirm = null) =>
        plans.Add((new GraphQLRequest { Query = query, Variables = variables }, fields.ToList(), guard, confirm));
    public string SourceValue(IdentifiedContentRequest patch) => FindSource(sources[patch.ResourceId], patch).Value<string>("value")!;
    public static PrimaryLocaleFieldResult Field(IdentifiedContentRequest patch, string reason = "") =>
        new() { ResourceId = patch.ResourceId, Key = patch.Key, Reason = reason };
}
