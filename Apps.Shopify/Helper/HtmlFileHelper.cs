using System.Text;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Filters.Bilingual.Xliff2;
using Blackbird.Filters.Transformations;

namespace Apps.Shopify.Helper;

public static class HtmlFileHelper
{
    public static string GetHtml(string content, string fileName)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        if (!Xliff2Serializer.IsXliff2(stream, out _))
            return content;

        stream.Position = 0;

        var transformation = Transformation.Load(stream, fileName);
        if (!transformation.Success)
            throw new PluginMisconfigurationException(transformation.Error);

        var target = transformation.Value.Target();
        if (!target.Success)
            throw new PluginMisconfigurationException(target.Error);

        using var reader = new StreamReader(target.Value.ToStream());
        return reader.ReadToEnd();
    }
}
