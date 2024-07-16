using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Request.OnlineStore;

public class StoreContentRequest
{
    [Display("Include themes")]
    public bool? IncludeThemes { get; set; }
    
    [Display("Include menu")]
    public bool? IncludeMenu { get; set; }
    
    [Display("Include shop")]
    public bool? IncludeShop { get; set; }
    
    [Display("Include shop policy")]
    public bool? IncludeShopPolicy { get; set; }
}