using Blackbird.Applications.Sdk.Common;

namespace Apps.Shopify.Models.Response.SellingPlan;

public record SearchSellingPlansResponse([property: Display("Selling plans")] List<SellingPlanResponse> Plans);