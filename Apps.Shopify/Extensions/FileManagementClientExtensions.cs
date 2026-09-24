using System.Text;
using Apps.Shopify.Helper;
using Apps.Shopify.Models.Dto;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;

namespace Apps.Shopify.Extensions;

public static class FileManagementClientExtensions
{
    public static async Task<FileReference> UploadFileRecord(this IFileManagementClient fileManagementClient, FileRecord fileRecord) 
    {
        return await fileManagementClient.UploadAsync(fileRecord.Stream, fileRecord.MimeType, fileRecord.FileName);
    }

    public static async Task<string> DownloadHtml(this IFileManagementClient fileManagementClient, FileReference inputFile)
    {
        var file = await fileManagementClient.DownloadAsync(inputFile);
        var fileBytes = await file.GetByteData();
        
        string fileContent = Encoding.UTF8.GetString(fileBytes);
        return HtmlFileHelper.GetHtml(fileContent, inputFile.Name);
    }
}