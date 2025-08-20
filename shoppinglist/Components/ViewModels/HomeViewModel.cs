using System;
using System.Threading.Tasks;
using Shoppinglist.Data.Models;
using shoppinglist.Models;
using shoppinglist.Services;

namespace shoppinglist.Components.Pages;

public class HomeViewModel : IDisposable
{
    private readonly ShoppingListService _service;
    private readonly AppEvents _events;

    public Items Items { get; } = new();
    public string NewName { get; set; } = "";
    public string Status { get; private set; } = "";
    
    public event Action Changed;
    private void RaiseChanged() => Changed?.Invoke();

    public HomeViewModel(ShoppingListService service, AppEvents events)
    {
        _service = service;
        _events = events;
        
        _events.ItemsChanged += OnItemsChanged;
    }

    public async Task InitializeAsync()
    {
        var loaded = await _service.GetAllAsync();
        Items.Set(loaded);
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
            RaiseChanged();
        }
    }

    public async Task ToggleAsync(Item item)
    {
        await _service.SetCheckedAsync(item.Id, item.IsChecked);
        Items.NotifyToggled(item);
        RaiseChanged();
    }

    public async Task DeleteAsync(int id)
    {
        await _service.DeleteAsync(id);
        Items.RemoveById(id);
        RaiseChanged();
    }

    private async void OnItemsChanged()
    {
        var updated = await _service.GetAllAsync();
        Items.Set(updated);
        RaiseChanged();
    }
    
    public void Dispose()
    {
        _events.ItemsChanged -= OnItemsChanged;
    }
}