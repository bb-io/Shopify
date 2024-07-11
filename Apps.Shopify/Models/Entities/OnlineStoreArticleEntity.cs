using Apps.Shopify.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Shopify.Models.Entities;

public class OnlineStoreArticleEntity : IRestPaginationEntity
{
    [Display("Article ID")]
    [JsonProperty("admin_graphql_api_id")]
    public string Id { get; set; }

    public string Title { get; set; }

    [Display("Created at")] public DateTime CreatedAt { get; set; }

    [Display("Updated at")] public DateTime UpdatedAt { get; set; }

    public string Author { get; set; }

    [Display("Blog ID")] public string BlogId { get; set; }

    public string Handle { get; set; }
}