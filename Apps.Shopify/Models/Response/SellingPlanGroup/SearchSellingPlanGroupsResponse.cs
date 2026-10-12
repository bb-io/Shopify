using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.SellingPlanGroup;

public record SearchSellingPlanGroupsResponse([property: Display("Selling plan groups")] List<SellingPlanGroupResponse> Groups);