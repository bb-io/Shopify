using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Response.Content;

public class ContentResponse(string contentId, string contentType) : IDownloadContentInput
{
    public string ContentId { get; set; } = contentId;
    public string ContentType { get; set; } = contentType;
}
