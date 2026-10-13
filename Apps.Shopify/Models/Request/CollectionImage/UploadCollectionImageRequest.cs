using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.CollectionImage;

public class UploadCollectionImageRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Collection image ID"), DataSource(typeof(CollectionImageDataHandler))]
    public string? CollectionImageId { get; set; }
}