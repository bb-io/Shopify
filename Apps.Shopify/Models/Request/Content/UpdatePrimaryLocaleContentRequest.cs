using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Content;

public class UpdatePrimaryLocaleContentRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;

    [Display("Content type"), StaticDataSource(typeof(PrimaryLocaleContentTypeDataHandler))]
    public string? ContentType { get; set; }

    [Display("Expected primary locale")]
    public string? ExpectedPrimaryLocale { get; set; }

    [Display("Dry run")]
    public bool? DryRun { get; set; } = true;

    [Display("Validate source digest")]
    public bool? ValidateSourceDigest { get; set; } = true;

    [Display("Skip unsupported fields")]
    public bool? SkipUnsupportedFields { get; set; } = false;
}
