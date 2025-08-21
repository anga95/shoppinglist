using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppinglist.Data.Models;
using shoppinglist.Services;

namespace shoppinglist.Components.ViewModels;

public class RecipiesMenuViewModel : ViewModelBase
{
    private readonly RecipieService _recipies;
    private readonly ShoppingListService _shoppingList;
    private readonly AppEvents _events;

    public List<Recipie> Recipies { get; private set; } = new();
    public HashSet<int> Expanded { get; } = new();
    public bool Open { get; private set; }

    public RecipiesMenuViewModel(RecipieService recipies, ShoppingListService shopping, AppEvents events) : base()
    {
        _recipies = recipies;
        _shoppingList = shopping;
        _events = events;
        _events.RecipiesChanged += OnEventsChanged;
        _events.ItemsChanged += OnEventsChanged;
    }

    private async Task OnEventsChanged() => await Reload();

    public async Task InitAsync()
    {
        Recipies = await _recipies.GetAllWithItemsAsync();
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

    private async Task Reload()
    {
        Recipies = await _recipies.GetAllWithItemsAsync();
        RaiseChanged();
    }
    
    public override void Dispose()
    {
        _events.RecipiesChanged -= OnEventsChanged;
        _events.ItemsChanged -= OnEventsChanged;
        base.Dispose();
    }
}