using Apps.Shopify.Models.Filters;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Shopify.Helper;

public static class ValidatorHelper
{
    public static void ValidateDates(this IDateFilter input)
    {
        List<string> errors = [];

        if (input is ICreatedDateFilter c &&
            c.CreatedAfter.HasValue && c.CreatedBefore.HasValue &&
            c.CreatedAfter > c.CreatedBefore)
        {
            errors.Add("'Created after' date cannot be later than 'Created before' date");
        }

        if (input is IUpdatedDateFilter u &&
            u.UpdatedAfter.HasValue && u.UpdatedBefore.HasValue &&
            u.UpdatedAfter > u.UpdatedBefore)
        {
            errors.Add("'Updated after' date cannot be later than 'Updated before' date");
        }

        if (input is IPublishedDateFilter p &&
            p.PublishedAfter.HasValue && p.PublishedBefore.HasValue &&
            p.PublishedAfter > p.PublishedBefore)
        {
            errors.Add("'Published after' date cannot be later than 'Published before' date");
        }

        if (errors.Count() > 0)
            throw new PluginMisconfigurationException(string.Join(". ", errors));
    }
}
