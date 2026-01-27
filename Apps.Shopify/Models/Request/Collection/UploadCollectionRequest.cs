using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.Collection;

public class UploadCollectionRequest
{
    [Display("Content")]
    public FileReference File { get; set; }

    [Display("Collection ID"), DataSource(typeof(CollectionDataHandler))]
    public string? CollectionId { get; set; }
}
