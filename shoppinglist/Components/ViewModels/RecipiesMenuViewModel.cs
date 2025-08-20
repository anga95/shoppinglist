using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shoppinglist.Data.Models;
using shoppinglist.Services;

public class RecipiesMenuViewModel
{
    private readonly RecipieService _recipies;
    private readonly ShoppingListService _shoppingList;
    private readonly AppEvents _events;

    public List<Recipie> List { get; private set; } = new();
    public HashSet<int> Expanded { get; } = new();
    public bool Open { get; private set; }

    public event Action? Changed;
    void RaiseChanged() => Changed?.Invoke();

    public RecipiesMenuViewModel(RecipieService recipies, ShoppingListService shopping, AppEvents events)
    {
        _recipies = recipies;
        _shoppingList = shopping;
        _events = events;
        _events.RecipiesChanged += async () => await Reload();
        _events.ItemsChanged += async () => await Reload();
    }

    public async Task InitAsync()
    {
        List = await _recipies.GetAllWithItemsAsync();
        RaiseChanged();
    }

    public void ToggleMenu() { Open = !Open; RaiseChanged(); }
    public void CloseMenu() { Open = false; RaiseChanged(); }

    public void ToggleRec(int id)
    {
        if (!Expanded.Add(id)) Expanded.Remove(id);
        RaiseChanged();
    }

    public async Task SetIngredientCheckedAsync(Item item)
    {
        await _shoppingList.SetCheckedAsync(item.Id, item.IsChecked);
        await Reload();
    }

    public async Task Reload()
    {
        List = await _recipies.GetAllWithItemsAsync();
        RaiseChanged();
    }
}