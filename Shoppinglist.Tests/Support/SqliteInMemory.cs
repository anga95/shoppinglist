using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data;

namespace Shoppinglist.Tests.Support;

public static class SqliteInMemory
{
    public static (ShoppingListDbContext ctx, SqliteConnection conn) CreateContext()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        
        var options = new DbContextOptionsBuilder<ShoppingListDbContext>()
            .UseSqlite(conn)
            .EnableSensitiveDataLogging()
            .Options;
        
        var ctx = new ShoppingListDbContext(options);
        ctx.Database.EnsureCreated();
        return (ctx, conn);
    }
}