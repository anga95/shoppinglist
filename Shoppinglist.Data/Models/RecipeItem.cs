namespace Shoppinglist.Data.Models;

public class RecipeItem
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}
