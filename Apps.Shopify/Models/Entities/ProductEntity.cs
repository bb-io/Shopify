using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Entities;

public class ProductEntity
{
    [Display("Product ID")]
    public string Id { get; set; }
    
    public string Title { get; set; }  
    
    public string Description { get; set; }   
    
    public string Handle { get; set; }   
    
    public string Status { get; set; }   
    
    [Display("Product type")]
    public string ProductType { get; set; }   
    
    [Display("Online store URL")]
    public string OnlineStoreUrl { get; set; }   
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }   
    
    [Display("Published at")]
    public DateTime? PublishedAt { get; set; }

    public IEnumerable<OptionsEntity>? Options { get; set; }
}