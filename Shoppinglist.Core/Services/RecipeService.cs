using Microsoft.EntityFrameworkCore;
using Shoppinglist.Core.Events;
using Shoppinglist.Data;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Core.Services;

public class RecipeService
{
    private readonly ShoppingListDbContext _db;
    private readonly IAppEvents _events;
    public RecipeService(ShoppingListDbContext db, IAppEvents events)
    {
        _db = db;
        _events = events;
    }
    
    public Task<List<Recipe>> GetAllWithItemsAsync() =>
        _db.Recipes
            .Include(r => r.RecipeItems)
            .ThenInclude(ri => ri.Item)
            .OrderBy(r => r.Title)
            .ToListAsync();

    public async Task CreateWithItemsAsync(string title, IEnumerable<string> itemNames)
    {
        title = title.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");
        
        Recipe recipe = new Recipe {Title = title};
        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        
        var names = itemNames
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            await _events.RaiseRecipesChangedAsync();
            return;
        }
        
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
        
        var linksToAdd = new List<RecipeItem>();
        foreach (var item in allItems)
        {
            bool exists = await _db.RecipeItems
                .AnyAsync(ri => ri.RecipeId == recipe.Id && ri.ItemId == item.Id);
                                
            
            if (!exists)
                linksToAdd.Add(new RecipeItem
                {
                    RecipeId = recipe.Id,
                    ItemId = item.Id
                });
        }

        if (linksToAdd.Count > 0)
        {
            _db.RecipeItems.AddRange(linksToAdd);
            await _db.SaveChangesAsync();
        }
        await _events.RaiseRecipesChangedAsync();
    }
    
    public async Task DeleteAsync(int recipeId)
    {
        Recipe? recipie = await _db.Recipes
            .Include(x => x.RecipeItems)
            .FirstOrDefaultAsync(x => x.Id == recipeId);
        if (recipie is null) return;
        
        _db.Recipes.Remove(recipie);
        await _db.SaveChangesAsync();
        await _events.RaiseRecipesChangedAsync();
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