using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data;
using Shoppinglist.Data.Models;

namespace shoppinglist.Services;

public class ShoppingListService
{
    private readonly ShoppingListDbContext _db;
    private readonly AppEvents _events;
    public ShoppingListService(ShoppingListDbContext db, AppEvents events)
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
        if (await _db.Items.AnyAsync(x => x.Name == trimmed)) return null;

        var item = new Item { Name = trimmed, IsChecked = false };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        await _events.RaiseItemsChanged();
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
            await _events.RaiseItemsChanged();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return;
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        await _events.RaiseItemsChanged();
    }
}