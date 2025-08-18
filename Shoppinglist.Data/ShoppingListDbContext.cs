using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Data;

public class ShoppingListDbContext : DbContext
{
    public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Recipie> Recipies => Set<Recipie>();
    public DbSet<RecipieItem> RecipieItems => Set<RecipieItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Name)
            .IsUnique();

        modelBuilder.Entity<RecipieItem>()
            .HasKey(ri => new { ri.RecipieId, ri.ItemId });

        modelBuilder.Entity<RecipieItem>()
            .HasOne(ri => ri.Recipie)
            .WithMany(r => r.RecipieItems)
            .HasForeignKey(ri => ri.RecipieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipieItem>()
            .HasOne(ri => ri.Item)
            .WithMany()
            .HasForeignKey(ri => ri.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}