namespace Apps.Shopify.Polling.Models.Memory;

public class DigestDateMemory
{
    public DateTime? LastInteractionDate { get; set; }
    
    ///<summary>
    /// Content ID -> composite digest hash for types with no updatedAt (such as menus)
    /// </summary>
    public Dictionary<string, string> Digests { get; set; } = new();
}