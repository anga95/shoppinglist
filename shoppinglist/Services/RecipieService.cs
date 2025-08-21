using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data;
using Shoppinglist.Data.Models;

namespace shoppinglist.Services;

public class RecipieService
{
    private readonly ShoppingListDbContext _db;
    private readonly AppEvents _events;
    public RecipieService(ShoppingListDbContext db, AppEvents events)
    {
        _db = db;
        _events = events;
    }
    
    public Task<List<Recipie>> GetAllWithItemsAsync() =>
        _db.Recipies
            .Include(r => r.RecipieItems)
            .ThenInclude(ri => ri.Item)
            .OrderBy(r => r.Title)
            .ToListAsync();

    public async Task CreateWithItemsAsync(string title, IEnumerable<string> itemNames)
    {
        title = title.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");
        
        Recipie recipie = new Recipie {Title = title};
        _db.Recipies.Add(recipie);
        await _db.SaveChangesAsync();
        
        var names = itemNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0) return;
        
        var existing = await _db.Items
            .Where(i => names.Contains(i.Name))
            .ToListAsync();
        
        var existingNames = existing.Select(n => n.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newItems = names
            .Where(n => !existingNames.Contains(n))
            .Select(n => new Item { Name = n, IsChecked = false, MovedAt = DateTime.UtcNow })
            .ToList();

        if (newItems.Count > 0)
        {
            _db.Items.AddRange(newItems);
            await _db.SaveChangesAsync();
        }

        var allItems = existing.Concat(newItems).ToList();
        
        var linksToAdd = new List<RecipieItem>();
        foreach (var item in allItems)
        {
            bool exists = await _db.RecipieItems
                .AnyAsync(ri => ri.RecipieId == recipie.Id && ri.ItemId == item.Id);
                                
            
            if (!exists)
                linksToAdd.Add(new RecipieItem
                {
                    RecipieId = recipie.Id,
                    ItemId = item.Id
                });
        }

        if (linksToAdd.Count > 0)
        {
            _db.RecipieItems.AddRange(linksToAdd);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task DeleteAsync(int recipieId)
    {
        Recipie? recipie = await _db.Recipies
            .Include(x => x.RecipieItems)
            .FirstOrDefaultAsync(x => x.Id == recipieId);
        if (recipie is null) return;
        
        _db.Recipies.Remove(recipie);
        await _db.SaveChangesAsync();
        await _events.RaiseRecipiesChangedAsync();
    }

    public async Task SetItemCheckedAsync(int itemId, bool value)
    {
        var it = await _db.Items.FindAsync(itemId);
        if (it is null) return;
        
        it.IsChecked = value;
        it.MovedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _events.RaiseItemsChangedAsync();
    }
}