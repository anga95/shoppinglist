using System;
using System.Threading.Tasks;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Func<Task>? RecipiesChanged;
    public event Func<Task>? ItemsChanged;

    public Task RaiseRecipiesChangedAsync() => RecipiesChanged?.Invoke() ?? Task.CompletedTask;
    public Task RaiseItemsChangedAsync() => ItemsChanged?.Invoke() ?? Task.CompletedTask;
}