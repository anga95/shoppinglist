using Microsoft.EntityFrameworkCore;
using shoppinglist.Data;
using shoppinglist.Models;

namespace shoppinglist.Services;

public class ShoppingListService
{
    private readonly ShoppingListDbContext _db;
    public ShoppingListService(ShoppingListDbContext db) => _db = db;

    public Task<List<Item>> GetAllAsync() =>
        _db.Items.OrderBy(i => i.Name).ToListAsync();

    public async Task<Item?> AddAsync(string name)
    {
        var trimmed = name?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(trimmed)) return null;

        if (await _db.Items.AnyAsync(x => x.Name == trimmed)) return null;

        var item = new Item { Name = trimmed, IsChecked = false };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task SetCheckedAsync(int id, bool value)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return;
        item.IsChecked = value;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return;
        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
    }
}