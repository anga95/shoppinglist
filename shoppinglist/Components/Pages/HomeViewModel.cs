using shoppinglist.Models;
using shoppinglist.Services;

namespace shoppinglist.Pages;

public class HomeViewModel
{
    private readonly ShoppingListService _service;

    public Items Items { get; } = new();
    public string NewName { get; set; } = "";
    public string Status { get; private set; } = "";

    public HomeViewModel(ShoppingListService service)
    {
        _service = service;
    }

    public async Task InitializeAsync()
    {
        var loaded = await _service.GetAllAsync();
        Items.Set(loaded);
    }

    public async Task AddAsync()
    {
        var added = await _service.AddAsync(NewName);
        Status = added is null ? "Tomt eller dubblett." : $"Lade till '{added.Name}'.";
        if (added is not null)
        {
            NewName = "";
            Items.Add(added);
        }
    }

    public async Task ToggleAsync(Item item)
    {
        await _service.SetCheckedAsync(item.Id, item.IsChecked);
        Items.NotifyToggled(item);
    }

    public async Task DeleteAsync(int id)
    {
        await _service.DeleteAsync(id);
        Items.RemoveById(id);
    }
}