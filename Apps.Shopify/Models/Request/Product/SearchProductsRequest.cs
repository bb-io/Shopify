using Apps.Shopify.DataSourceHandlers;
using Apps.Shopify.DataSourceHandlers.Static;
using Apps.Shopify.Models.Filters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Models.Request.Product;

public class SearchProductsRequest : ICreatedDateFilter, IPublishedDateFilter, IUpdatedDateFilter
{
    [Display("Title contains")]
    public string? TitleContains { get; set; }

    [Display("Status"), StaticDataSource(typeof(ProductStatusHandler))]
    public string? Status { get; set; }
    
    [Display("Metafield key"), DataSource(typeof(ProductMetafieldKeyDataHandler))]
    public string? MetafieldKey { get; set; }
    
    [Display("Metafield value contains")]
    public string? MetafieldValueContains { get; set; }

    [Display("Published before")]
    public DateTime? PublishedBefore { get; set; }

    [Display("Published after")]
    public DateTime? PublishedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }

    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(MetafieldKey) ^ string.IsNullOrEmpty(MetafieldValueContains))
            throw new PluginMisconfigurationException("Metafield and Metafield value should be both either filled or empty");
    }
}