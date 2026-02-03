using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Shopify.Models.Request.Content;

public class UploadContentRequest : IUploadContentInput
{    
    [Display("Content")]
    public FileReference Content { get; set; }

    [Display("Content type"), StaticDataSource(typeof(ContentTypeDataHandler))]
    public string? ContentType { get; set; }

    [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
    public string? ContentId { get; set; }

    [Display("Locale"), DataSource(typeof(NonPrimaryLanguageDataHandler))]
    public string Locale { get; set; }
}