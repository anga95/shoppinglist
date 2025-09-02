using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Data;

public class ShoppingListDbContext : DbContext
{
    public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeItem> RecipeItems => Set<RecipeItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Name)
            .IsUnique();

        modelBuilder.Entity<RecipeItem>()
            .HasKey(ri => new { ri.RecipeId, ri.ItemId });

        modelBuilder.Entity<RecipeItem>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeItems)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeItem>()
            .HasOne(ri => ri.Item)
            .WithMany()
            .HasForeignKey(ri => ri.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
