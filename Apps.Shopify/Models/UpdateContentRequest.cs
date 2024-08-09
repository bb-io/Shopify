using Apps.Shopify.DataSourceHandlers.DictionaryHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Shopify.Models;

public class UpdateContentRequest
{
    [Display("Content type", Description = "Type of content to update"), StaticDataSource(typeof(ContentTypeDataHandler))] 
    public string? ContentType { get; set; } = string.Empty;
}