using System;
using System.Linq;
using System.Threading.Tasks;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Func<Task>? RecipiesChanged;
    public event Func<Task>? ItemsChanged;

    public Task RaiseRecipiesChangedAsync() => InvokeAsync(RecipiesChanged);
    public Task RaiseItemsChangedAsync() => InvokeAsync(ItemsChanged);

    // Invoke each subscribed handler and wait for all to complete. Using
    // GetInvocationList ensures that every subscriber runs even if one
    // throws, so multiple listeners are handled correctly.
    private static async Task InvokeAsync(Func<Task>? handlers)
    {
        if (handlers is null) return;
        var tasks = handlers.GetInvocationList()
            .Cast<Func<Task>>()
            .Select(h =>
            {
                try { return h(); }
                catch (Exception ex) { return Task.FromException(ex); }
            });
        await Task.WhenAll(tasks);
    }
}