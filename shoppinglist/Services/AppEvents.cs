using System;
using System.Linq;
using System.Threading.Tasks;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Func<Task>? RecipiesChanged;
    public event Func<Task>? ItemsChanged;

    public Task RaiseRecipiesChanged() => InvokeAsync(RecipiesChanged);
    public Task RaiseItemsChanged() => InvokeAsync(ItemsChanged);

    private static async Task InvokeAsync(Func<Task>? handlers)
    {
        if (handlers is null) return;
        foreach (var handler in handlers.GetInvocationList().Cast<Func<Task>>())
        {
            await handler();
        }
    }
}