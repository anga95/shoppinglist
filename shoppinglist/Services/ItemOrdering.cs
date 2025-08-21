using System;
using System.Collections.Generic;
using System.Linq;
using Shoppinglist.Data.Models;

namespace shoppinglist.Services;

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

