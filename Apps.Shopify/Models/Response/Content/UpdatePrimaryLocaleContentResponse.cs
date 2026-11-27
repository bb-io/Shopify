using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.Content;

public class PrimaryLocaleFieldResult
{
    [Display("Resource ID")]
    public string ResourceId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class UpdatePrimaryLocaleContentResponse
{
    [Display("Primary locale")]
    public string PrimaryLocale { get; set; } = string.Empty;
    [Display("Dry run")]
    public bool DryRun { get; set; }
    [Display("Prepared resources")]
    public int PreparedResources { get; set; }
    [Display("Prepared fields")]
    public List<PrimaryLocaleFieldResult> PreparedFields { get; set; } = [];
    [Display("Updated resources")]
    public int UpdatedResources => UpdatedResourceIds.Count;
    [Display("Updated fields")]
    public List<PrimaryLocaleFieldResult> UpdatedFields { get; set; } = [];
    [Display("Unchanged fields")]
    public List<PrimaryLocaleFieldResult> UnchangedFields { get; set; } = [];
    [Display("Skipped fields")]
    public List<PrimaryLocaleFieldResult> SkippedFields { get; set; } = [];
    [Display("Updated resource IDs")]
    public List<string> UpdatedResourceIds { get; set; } = [];
    [Display("Pending asynchronous jobs")]
    public List<string> PendingJobs { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
