using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppinglist.Data.Models;
using shoppinglist.Services;

namespace shoppinglist.Components.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly ShoppingListService _service;
    private readonly AppEvents _events;

    public List<Item> Items { get; private set; } = new();
    public string NewName { get; set; } = "";
    public string Status { get; private set; } = "";

    public HomeViewModel(ShoppingListService service, AppEvents events) : base()
    {
        _service = service;
        _events = events;
        _events.ItemsChanged += OnItemsChangedAsync;
    }

    public async Task InitializeAsync()
    {
        Items = OrderForDisplay(await _service.GetAllAsync()); 
        RaiseChanged();
    }

    public async Task AddAsync()
    {
        var added = await _service.AddAsync(NewName);
        Status = added is null ? "Tomt eller dubblett." : $"Lade till '{added.Name}'.";
        if (added is not null)
        {
            NewName = "";
            Items.Add(added);
            Items = OrderForDisplay(Items);
            RaiseChanged();
        }
    }

    public async Task ToggleAsync(Item item)
    {
        bool original = item.IsChecked;
        try
        {
            await _service.SetCheckedAsync(item.Id, item.IsChecked);
            Items = OrderForDisplay(Items);
        }
        catch (Exception ex)
        {
            item.IsChecked = original;
            Status = $"Fel vid uppdatering: {ex.Message}";
        }
        finally { RaiseChanged(); }
    }

    public async Task DeleteAsync(int id)
    {
        var backup = Items;
        Items = Items.Where(i => i.Id != id).ToList();
        RaiseChanged();

        try { await _service.DeleteAsync(id); }
        catch (Exception ex)
        {
            Items = backup;
            Status = $"Fel vid borttagning: {ex.Message}";
            RaiseChanged();
        }
    }

    private async Task OnItemsChangedAsync()
    {
        try
        {
            Items = OrderForDisplay(await _service.GetAllAsync());
            RaiseChanged();
        }
        catch (Exception ex)
        {
            Status = $"Fel vid uppdatering: {ex.Message}";
            RaiseChanged();
        }
    }

    private static List<Item> OrderForDisplay(IEnumerable<Item> src)
    {
        var items = src.OrderBy(i => i.IsChecked)
            .ThenBy(i => i.MovedAt)
            .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return items;
    }
    
    public override void Dispose()
    {
        _events.ItemsChanged -= OnItemsChangedAsync;
        base.Dispose();
    }
}