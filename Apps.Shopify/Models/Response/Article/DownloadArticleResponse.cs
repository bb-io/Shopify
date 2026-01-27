using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Response.Article;

public class DownloadArticleResponse(FileReference file)
{
    [Display("Article")]
    public FileReference File { get; set; } = file;
}
