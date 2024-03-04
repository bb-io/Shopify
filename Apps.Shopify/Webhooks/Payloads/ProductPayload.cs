using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Shopify.Webhooks.Payloads;

public class ProductPayload
{
    [Display("Product ID")]
    [JsonProperty("admin_graphql_api_id")]
    public string Id { get; set; }
    
    public string Title { get; set; }  
    
    [Display("Body HTML")]
    [JsonProperty("body_html")]
    public string BodyHtml { get; set; }
    
    public string Status { get; set; }  
    
    public string Handle { get; set; }   
    
    [Display("Product type")]
    [JsonProperty("product_type")]
    public string ProductType { get; set; }  
    
    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }   
    
    [Display("Published at")]
    [JsonProperty("published_at")]
    public DateTime? PublishedAt { get; set; }   
}