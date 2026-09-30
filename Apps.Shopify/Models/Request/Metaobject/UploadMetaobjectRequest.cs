using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Metaobject;

public class UploadMetaobjectRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Metaobject ID"), DataSource(typeof(MetaobjectDataHandler))]
    public string? MetaobjectId { get; set; }
}