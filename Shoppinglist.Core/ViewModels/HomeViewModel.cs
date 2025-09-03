using Shoppinglist.Core.Events;
using Shoppinglist.Core.Services;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Core.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly ShoppingListService _service;
    private readonly IAppEvents _events;

    public List<Item> Items { get; private set; } = new();
    public string NewName { get; set; } = "";
    public string Status { get; private set; } = "";

    public HomeViewModel(ShoppingListService service, IAppEvents events) : base()
    {
        _service = service;
        _events = events;
        _events.ItemsChanged += OnItemsChangedAsync;
    }

    public async Task InitializeAsync()
    {
        Items = (await _service.GetAllAsync()).OrderForDisplay().ToList();
        RaiseChanged();
    }

    public async Task AddAsync()
    {
        var added = await _service.AddAsync(NewName);

        if (added is null)
        {
            await ShowStatusAsync("Tomt namn.");
        }
        else
        {
            NewName = "";
            await ShowStatusAsync($"'{added.Name}' tillagd.");
        }
    }

    public async Task ToggleAsync(Item item)
    {
        bool original = item.IsChecked;
        try
        {
            await _service.SetCheckedAsync(item.Id, item.IsChecked);
            Items = Items.OrderForDisplay().ToList();
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
            Items = (await _service.GetAllAsync()).OrderForDisplay().ToList();
            RaiseChanged();
        }
        catch (Exception ex)
        {
            Status = $"Fel vid uppdatering: {ex.Message}";
            RaiseChanged();
        }
    }

    public async Task ShowStatusAsync(string message, int durationMs = 2000)
    {
        Status = message;
        RaiseChanged();
        await Task.Delay(durationMs);
        Status = "";
        RaiseChanged();
    }
    
    public override void Dispose()
    {
        _events.ItemsChanged -= OnItemsChangedAsync;
        base.Dispose();
    }
}