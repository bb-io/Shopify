using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Models.Request.OnlineStore;

public class DownloadStoreContentRequest
{
    [Display("Include themes")]
    public bool? IncludeThemes { get; set; }
    
    [Display("Include menu")]
    public bool? IncludeMenu { get; set; }
    
    [Display("Include shop")]
    public bool? IncludeShop { get; set; }
    
    [Display("Include shop policy")]
    public bool? IncludeShopPolicy { get; set; }

    public void Validate()
    {
        if (!HasItemsIncluded())
            throw new PluginMisconfigurationException("You should include at least one content type. Please check your input and try again");
    }
    
    private bool HasItemsIncluded()
    {
        return 
            IncludeThemes is true ||
            IncludeMenu is true ||
            IncludeShop is true ||
            IncludeShopPolicy is true;
    }
}