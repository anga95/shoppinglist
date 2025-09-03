using Microsoft.EntityFrameworkCore;
using Shoppinglist.Core.Events;
using Shoppinglist.Data;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Core.Services;

public class ShoppingListService
{
    private readonly ShoppingListDbContext _db;
    private readonly IAppEvents _events;
    public ShoppingListService(ShoppingListDbContext db, IAppEvents events)
    {
        _db = db;
        _events = events;
    }

    public Task<List<Item>> GetAllAsync() =>
        _db.Items
            .OrderForDisplay()
            .ToListAsync();

    public async Task<Item?> AddAsync(string name)
    {
        var trimmed = name?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(trimmed)) return null;
        
        var itemAlreadyExists = await _db.Items.FirstOrDefaultAsync(x => x.Name == trimmed);
        if (itemAlreadyExists is not null)
        {
            if (itemAlreadyExists.IsChecked)
            {
                itemAlreadyExists.IsChecked = false;
                itemAlreadyExists.MovedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                await _events.RaiseItemsChangedAsync();
            }
            return itemAlreadyExists;
        }

        var item = new Item { Name = trimmed, IsChecked = false };
        _db.Items.Add(item);
        
        await _db.SaveChangesAsync();
        await _events.RaiseItemsChangedAsync();
        return item;
    }

    public async Task SetCheckedAsync(int id, bool value)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return;

        if (item.IsChecked != value)
        {
            item.IsChecked = value;
            item.MovedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _events.RaiseItemsChangedAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return;
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        await _events.RaiseItemsChangedAsync();
    }
}