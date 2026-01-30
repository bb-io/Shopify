using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.OnlineStoreArticle;

public class UploadArticleRequest
{
    [Display("Content")]
    public FileReference File { get; set; }
}
