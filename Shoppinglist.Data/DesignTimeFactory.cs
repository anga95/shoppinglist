using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shoppinglist.Data;

public class DesignTimeFactory : IDesignTimeDbContextFactory<ShoppingListDbContext>
{
    public ShoppingListDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<ShoppingListDbContext>()
            .UseSqlite("Data Source=shopping.design.db")
            .Options;

        return new ShoppingListDbContext(opts);
    }
}