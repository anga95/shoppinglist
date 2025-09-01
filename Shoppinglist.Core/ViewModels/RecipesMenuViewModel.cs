using Shoppinglist.Core.Events;
using Shoppinglist.Core.Services;
using Shoppinglist.Data.Models;

namespace Shoppinglist.Core.ViewModels;

public class RecipesMenuViewModel : ViewModelBase
{
    private readonly RecipeService _recipes;
    private readonly ShoppingListService _shoppingList;
    private readonly IAppEvents _events;

    public List<Recipe> Recipes { get; private set; } = new();
    public HashSet<int> Expanded { get; } = new();
    public bool Open { get; private set; }

    public RecipesMenuViewModel(RecipeService recipes, ShoppingListService shopping, IAppEvents events) : base()
    {
        _recipes = recipes;
        _shoppingList = shopping;
        _events = events;
        _events.RecipesChanged += OnEventsChanged;
        _events.ItemsChanged += OnEventsChanged;
    }

    private Task OnEventsChanged() => Reload();

    public async Task InitAsync()
    {
        Recipes = await _recipes.GetAllWithItemsAsync();
        RaiseChanged();
    }

    public void ToggleMenu() { Open = !Open; RaiseChanged(); }
    public void CloseMenu() { Open = false; RaiseChanged(); }

    public void ToggleRec(int id)
    {
        if (!Expanded.Add(id)) Expanded.Remove(id);
        RaiseChanged();
    }

    public async Task SetIngredientCheckedAsync(Item item)
    {
        await _shoppingList.SetCheckedAsync(item.Id, item.IsChecked);
        await Reload();
    }

    private async Task Reload()
    {
        Recipes = await _recipes.GetAllWithItemsAsync();
        RaiseChanged();
    }
    
    public override void Dispose()
    {
        _events.RecipesChanged -= OnEventsChanged;
        _events.ItemsChanged -= OnEventsChanged;
        base.Dispose();
    }
}