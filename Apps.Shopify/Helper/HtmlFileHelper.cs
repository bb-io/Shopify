using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;

namespace Apps.Shopify.Helper;

public static class HtmlFileHelper
{
    public static string GetHtml(string content, string fileName)
    {
        if (Xliff2Serializer.IsXliff2(content))
        {
            content = Transformation.Parse(content, fileName).Target().Serialize() ??
                      throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }

        return content;
    }
}
