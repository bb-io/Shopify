namespace Apps.Shopify.Models.Request.TranslatableResource;

public class IdentifiedContentRequest : TranslatableResourceContentRequest
{
    public string ResourceId { get; set; }
    
    public void SetResourceId(string resourceId)
    {
        if (ResourceId == resourceId)
            return;

        ResourceId = resourceId;
        TranslatableContentDigest = string.Empty;
    }
}