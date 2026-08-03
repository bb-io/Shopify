using Apps.Shopify.Models.Entities.Market;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Market;

public class MarketPaginationResponse : IPaginationResponse<MarketEntity>
{
    [JsonProperty("markets")] 
    public PaginationData<MarketEntity> Items { get; set; }
}
