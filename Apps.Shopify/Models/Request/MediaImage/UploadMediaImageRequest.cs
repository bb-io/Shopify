using Apps.Shopify.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Models.Request.MediaImage;

public class UploadMediaImageRequest
{
    [Display("Content")]
    public FileReference File { get; set; } = null!;

    [Display("Media image ID"), DataSource(typeof(MediaImageDataHandler))]
    public string? MediaImageId { get; set; }
}