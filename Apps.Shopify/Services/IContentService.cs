using Apps.Shopify.Models.Dto;
using Apps.Shopify.Models.Request.Content;
using Apps.Shopify.Models.Response.Content;
using Apps.Shopify.Services.Models;

namespace Apps.Shopify.Services;

public interface IContentService
{
    Task<FileRecord> Download(DownloadContentRequest input);
    Task Upload(UploadContentServiceRequest input);
    Task<SearchContentResponse> Search(SearchContentRequest input);
}
