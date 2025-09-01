using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppinglist.Data;
using Shoppinglist.Data.Models;
using shoppinglist.Services;
using Xunit;

public class ShoppingListServiceTests
{
    private static ShoppingListService CreateService(out AppEvents events)
    {
        var options = new DbContextOptionsBuilder<ShoppingListDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new ShoppingListDbContext(options);
        events = new AppEvents();
        return new ShoppingListService(db, events);
    }

    [Fact]
    public async Task AddAsync_Adds_New_Item_And_Raises_Event()
    {
        var service = CreateService(out var events);
        bool raised = false;
        events.ItemsChanged += () => { raised = true; return Task.CompletedTask; };

        var item = await service.AddAsync(" Milk ");

        Assert.NotNull(item);
        Assert.Equal("Milk", item!.Name);
        Assert.True(raised);

        var all = await service.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task AddAsync_Rejects_Duplicate()
    {
        var service = CreateService(out var _);
        await service.AddAsync("Eggs");

        var item = await service.AddAsync("Eggs");

        Assert.Null(item);
        var all = await service.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task SetCheckedAsync_Toggles_State_And_Updates_MovedAt()
    {
        var service = CreateService(out var _);
        var item = await service.AddAsync("Bread");

        DateTime? before = item!.MovedAt;
        await service.SetCheckedAsync(item.Id, true);

        var updated = (await service.GetAllAsync()).Single();
        Assert.True(updated.IsChecked);
        Assert.NotEqual(before, updated.MovedAt);
    }

    [Fact]
    public async Task DeleteAsync_Removes_Item()
    {
        var service = CreateService(out var _);
        var item = await service.AddAsync("Banana");

        await service.DeleteAsync(item!.Id);
        var all = await service.GetAllAsync();
        Assert.Empty(all);
    }

    [Fact]
    public void OrderForDisplay_Sorts_Items()
    {
        var now = DateTime.UtcNow;
        var items = new[]
        {
            new Item { Name = "Bananas", IsChecked = false, MovedAt = now },
            new Item { Name = "Apples", IsChecked = false, MovedAt = now },
            new Item { Name = "Carrots", IsChecked = true,  MovedAt = now.AddMinutes(-1) },
            new Item { Name = "Dates",   IsChecked = true,  MovedAt = now }
        };

        var ordered = items.OrderForDisplay().ToList();

        Assert.Equal(new[] { "Apples", "Bananas", "Carrots", "Dates" }, ordered.Select(i => i.Name).ToArray());
    }
}
