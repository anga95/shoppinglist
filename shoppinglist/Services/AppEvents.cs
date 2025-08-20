using System;

namespace shoppinglist.Services;

public class AppEvents
{
    public event Action? RecipiesChanged;
    public event Action? ItemsChanged;
    public void RaiseRecipiesChanged() => RecipiesChanged?.Invoke();
    public void RaiseItemsChanged() => ItemsChanged?.Invoke();
}