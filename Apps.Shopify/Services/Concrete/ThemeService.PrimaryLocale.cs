using Apps.Shopify.Constants.GraphQL;
using Apps.Shopify.Models.Request.TranslatableResource;
using Apps.Shopify.Services.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json.Linq;

namespace Apps.Shopify.Services.Concrete;

public partial class ThemeService : IPrimaryLocaleContentService
{
    private static async Task<List<JToken>> ReadPrimaryLocaleFiles(PrimaryLocaleUpdateContext context, string id)
    {
        var files = new List<JToken>();
        string? after = null;
        do
        {
            JObject data;
            try { data = await context.Query(PrimaryLocaleGraphQl.ThemeFiles, new { id, after }); }
            catch (PluginApplicationException ex)
            { throw new PluginApplicationException($"Cannot read theme files. Verify read_themes access. Shopify: {ex.Message}"); }
            if (data["theme"]?.Type != JTokenType.Object)
                throw PrimaryLocaleUpdateContext.Invalid($"Theme no longer exists or is inaccessible: {id}");
            var connection = data["theme"]!["files"];
            if (connection?.Type != JTokenType.Object) throw PrimaryLocaleUpdateContext.Invalid("Theme files are unavailable; verify read_themes permission");
            files.AddRange(connection["nodes"]!);
            if (connection["pageInfo"]!.Value<bool>("hasNextPage"))
            {
                var cursor = connection["pageInfo"]!.Value<string>("endCursor");
                if (cursor == null || cursor == after) throw PrimaryLocaleUpdateContext.Invalid("Invalid theme file pagination cursor");
                after = cursor;
            }
            else break;
        } while (true);
        if (files.GroupBy(x => x.Value<string>("filename")).Any(x => x.Count() > 1))
            throw PrimaryLocaleUpdateContext.Invalid("Ambiguous theme filenames");
        return files;
    }

    public async Task PreparePrimaryLocaleUpdate(PrimaryLocaleUpdateContext context, IReadOnlyList<IdentifiedContentRequest> patches)
    {
        foreach (var group in patches.GroupBy(x => x.ResourceId))
        {
            if (!group.Key.StartsWith("gid://shopify/OnlineStoreTheme/", StringComparison.Ordinal))
                throw PrimaryLocaleUpdateContext.Invalid($"Expected original OnlineStoreTheme GID: {group.Key}");
            var files = await ReadPrimaryLocaleFiles(context, group.Key);
            var mapped = new List<(JToken File, ThemeJsonPatch.Location Location, IdentifiedContentRequest Patch)>();
            var locations = new List<(JToken File, ThemeJsonPatch.Location Location)>();
            foreach (var file in files)
            {
                var filename = file.Value<string>("filename")!;
                if (!filename.StartsWith("locales/", StringComparison.Ordinal) || !filename.EndsWith(".default.json", StringComparison.Ordinal))
                    throw PrimaryLocaleUpdateContext.Invalid("Unexpected asset in default locale file query");
                var content = file["body"]?.Value<string>("content");
                if (content == null) throw PrimaryLocaleUpdateContext.Invalid($"Theme file is not a text asset: {filename}");
                try { locations.AddRange(ThemeJsonPatch.Locations(content).Select(x => (file, x))); }
                catch (System.Text.Json.JsonException ex)
                { throw PrimaryLocaleUpdateContext.Invalid($"Unsupported theme JSON format in '{filename}': {ex.Message}"); }
            }
            foreach (var patch in group)
            {
                // Exact flattened locale key only. Never infer template/section paths from a prefix.
                var matches = locations.Where(x => x.Location.Key == patch.Key).ToList();
                if (matches.Count != 1)
                {
                    context.Unsupported(patch, "No unique exact key in a default storefront locale JSON asset. Template, schema, settings and Liquid paths cannot be inferred safely.");
                    continue;
                }
                var match = matches[0];
                if (match.Location.Value != context.SourceValue(patch))
                { context.Unsupported(patch, "Asset value does not match Shopify original source; asset mapping cannot be verified"); continue; }
                mapped.Add((match.File, match.Location, patch));
            }
            foreach (var asset in mapped.GroupBy(x => x.File.Value<string>("filename")!))
            {
                var file = asset.First().File;
                var original = file["body"]!.Value<string>("content")!;
                var merged = ThemeJsonPatch.Apply(original, asset.Select(x => (x.Location, x.Patch.Value)));
                context.Add(PrimaryLocaleGraphQl.ThemeFilesUpsert, new
                {
                    themeId = group.Key,
                    files = new[] { new { filename = asset.Key, body = new { type = "TEXT", value = merged } } }
                }, asset.Select(x => x.Patch), async () =>
                {
                    // Latest complete asset must still match the version used to build the merge.
                    var latest = (await ReadPrimaryLocaleFiles(context, group.Key)).SingleOrDefault(x => x.Value<string>("filename") == asset.Key);
                    if (latest == null || latest["body"]?.Value<string>("content") != original ||
                        latest.Value<string>("checksumMd5") != file.Value<string>("checksumMd5"))
                        throw PrimaryLocaleUpdateContext.Invalid($"Theme asset changed before write: {asset.Key}. Run again to merge the latest asset.");
                }, async () =>
                {
                    var latest = (await ReadPrimaryLocaleFiles(context, group.Key)).SingleOrDefault(x => x.Value<string>("filename") == asset.Key);
                    return latest?["body"]?.Value<string>("content") == merged;
                });
            }
        }
    }
}
