using System;
using System.Linq;
using System.Threading.Tasks;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Func<Task>? RecipiesChanged;
    public event Func<Task>? ItemsChanged;

    public async Task RaiseRecipiesChangedAsync()
    {
        if (RecipiesChanged is { } handlers)
        {
            var tasks = handlers.GetInvocationList()
                .Cast<Func<Task>>()
                .Select(h => h());
            await Task.WhenAll(tasks);
        }
    }

    public async Task RaiseItemsChangedAsync()
    {
        if (ItemsChanged is { } handlers)
        {
            var tasks = handlers.GetInvocationList()
                .Cast<Func<Task>>()
                .Select(h => h());
            await Task.WhenAll(tasks);
        }
    }
}