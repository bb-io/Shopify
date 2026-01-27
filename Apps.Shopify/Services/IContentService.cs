using Apps.Shopify.Models.Request.Content;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Shopify.Services;

public interface IContentService
{
    Task<FileReference> Download(DownloadContentRequest input);
    Task Upload(UploadContentRequest input);
}
