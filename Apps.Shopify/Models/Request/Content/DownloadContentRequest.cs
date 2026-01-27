using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Request.Content;

public class DownloadContentRequest : IDownloadContentInput
{
    [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; }

    [DataSource(typeof(LanguageDataHandler))]
    public string Locale { get; set; }

    [Display("Outdated", Description = "False by default")]
    public bool? Outdated { get; set; }

    [Display("Include blog posts for downloaded blog")]
    public bool? IncludeBlogPosts { get; set; }

    [Display("Include product metafields")]
    public bool? IncludeMetafields { get; set; }

    [Display("Include product options")]
    public bool? IncludeOptions { get; set; }

    [Display("Include product option values")]
    public bool? IncludeOptionValues { get; set; }

    [Display("Asset keys", Description = "Specific store assets to translate"), DataSource(typeof(AssetThemeDataHandler))]
    public IEnumerable<string>? AssetKeys { get; set; }
}
