using Shoppinglist.Data.Models;

namespace Shoppinglist.Core.Services;

public static class ItemOrdering
{
    public static IEnumerable<Item> OrderForDisplay(this IEnumerable<Item> src) =>
        src.OrderBy(i => i.IsChecked)
            .ThenBy(i => i.MovedAt)
            .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase);

    public static IOrderedQueryable<Item> OrderForDisplay(this IQueryable<Item> src) =>
        src.OrderBy(i => i.IsChecked)
            .ThenBy(i => i.MovedAt)
            .ThenBy(i => i.Name);
}

