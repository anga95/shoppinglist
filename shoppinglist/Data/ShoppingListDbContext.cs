using Microsoft.EntityFrameworkCore;
using shoppinglist.Models;

namespace shoppinglist.Data;

public class ShoppingListDbContext : DbContext
{
    public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options)
        : base(options) { }

    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Name)
            .IsUnique();
    }
}