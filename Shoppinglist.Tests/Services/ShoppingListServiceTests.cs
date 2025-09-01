using FluentAssertions;
using Shoppinglist.Core.Events;
using Shoppinglist.Core.Services;
using Shoppinglist.Tests.Support;

namespace Shoppinglist.Tests.Services;

public class ShoppingListServiceTests
{
    [Fact]
    public async Task AddAsync_ValidName_AddsItem()
    {
        var (ctx, conn) = SqliteInMemory.CreateContext();
        await using var _ = conn;

        var eventBus = new AppEvents();
        var service = new ShoppingListService(ctx, eventBus);
        var added = await service.AddAsync("  Milk  ");
        
        added.Should().NotBeNull();
        added!.Name.Should().Be("Milk");

        var all = await service.GetAllAsync();
        all.Should().ContainSingle( i => i.Name == "Milk" && !i.IsChecked);


    }
}