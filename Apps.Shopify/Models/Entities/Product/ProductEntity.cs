using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities.Product;

public class ProductEntity
{
    [Display("Product ID")]
    public string Id { get; set; }
    
    [Display("Product title")]
    public string Title { get; set; }

    [Display("Description")]
    public string Description { get; set; }

    [Display("Handle")]
    public string Handle { get; set; }

    [Display("Status")]
    public string Status { get; set; }   
    
    [Display("Product type")]
    public string ProductType { get; set; }   
    
    [Display("Online store URL")]
    public string? OnlineStoreUrl { get; set; }   
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }

    [Display("Published at")]
    public DateTime? PublishedAt { get; set; }

    [Display("Metafield")]
    public ProductMetafield? Metafield { get; set; }

    [Display("Options")]
    public IEnumerable<ProductOption>? Options { get; set; }
}