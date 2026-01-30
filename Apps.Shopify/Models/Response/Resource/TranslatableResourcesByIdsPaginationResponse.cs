using Apps.Shopify.Models.Entities.Resource;
using Apps.Shopify.Models.Response.Pagination;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Response.TranslatableResource;

public class TranslatableResourcesByIdsPaginationResponse : IPaginationResponse<TranslatableResourceEntity>
{
    [JsonProperty("translatableResourcesByIds")]
    public PaginationData<TranslatableResourceEntity> Items { get; set; }
}