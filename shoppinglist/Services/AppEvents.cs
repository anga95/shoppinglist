using System;
using System.Threading.Tasks;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Func<Task>? RecipiesChanged;
    public event Func<Task>? ItemsChanged;
    public Task RaiseRecipiesChanged() => RecipiesChanged?.Invoke() ?? Task.CompletedTask;
    public Task RaiseItemsChanged() => ItemsChanged?.Invoke() ?? Task.CompletedTask;
}