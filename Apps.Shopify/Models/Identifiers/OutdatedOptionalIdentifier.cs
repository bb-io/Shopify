using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Identifiers;

public class OutdatedOptionalIdentifier
{
    [Display("Outdated")]
    public bool? Outdated { get; set; }
}