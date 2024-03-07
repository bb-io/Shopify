using Apps.Shopify.Models.Entities;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.Event;

public class EventPaginationResponse : IPaginationResponse<EventEntity>
{
    [JsonProperty("webhookSubscriptions")]
    public PaginationData<EventEntity> Items { get; set; }
}