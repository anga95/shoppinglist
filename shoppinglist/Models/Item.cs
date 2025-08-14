namespace shoppinglist.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsChecked { get; set; }
}