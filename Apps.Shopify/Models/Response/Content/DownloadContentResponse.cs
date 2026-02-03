using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Response.Content;

public class DownloadContentResponse(FileReference content) : IDownloadContentOutput
{
    [Display("Content")]
    public FileReference Content { get; set; } = content;
}
