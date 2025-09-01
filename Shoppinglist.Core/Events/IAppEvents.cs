namespace Shoppinglist.Core.Events;

public interface IAppEvents
{
    event Func<Task>? RecipesChanged;
    event Func<Task>? ItemsChanged;
    
    Task RaiseRecipesChangedAsync();
    Task RaiseItemsChangedAsync();
}