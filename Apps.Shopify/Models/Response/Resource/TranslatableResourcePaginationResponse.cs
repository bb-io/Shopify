using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.TranslatableResource;

public class TranslatableResourcePaginationResponse : IPaginationResponse<TranslatableResourceEntity>
{
    [JsonProperty("translatableResources")]
    public PaginationData<TranslatableResourceEntity> Items { get; set; }
}