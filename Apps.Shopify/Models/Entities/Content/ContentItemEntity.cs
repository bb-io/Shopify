using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Entities.Content;

public class ContentItemEntity(string contentId, string contentType, string name) : IDownloadContentInput
{
    [Display("Content ID")]
    public string ContentId { get; set; } = contentId;

    [Display("Content type")]
    public string ContentType { get; set; } = contentType;

    [Display("Name")]
    public string Name { get; set; } = name;
}
