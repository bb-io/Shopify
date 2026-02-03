using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using System.Text;

namespace Apps.Shopify.Helper;

public static class HtmlFileHelper
{
    public static async Task<string> GetHtmlFromFile(IFileManagementClient fileManagement, FileReference reference)
    {
        var file = await fileManagement.DownloadAsync(reference);
        var html = Encoding.UTF8.GetString(await file.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, reference.Name).Target().Serialize() ??
                throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }

        return html;
    }
}
