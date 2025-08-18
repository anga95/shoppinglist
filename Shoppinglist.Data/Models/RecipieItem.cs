namespace Shoppinglist.Data.Models;

public class RecipieItem
{
    public int RecipieId { get; set; }
    public Recipie Recipie { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}