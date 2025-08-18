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
    public RecipieService(ShoppingListDbContext db) => _db = db;
    
    public Task<List<Recipie>> GetAllWithItemsAsync() =>
        _db.Recipies
            .Include(r => r.RecipieItems)
            .ThenInclude(ri => ri.Item)
            .OrderBy(r => r.Title)
            .ToListAsync();
}