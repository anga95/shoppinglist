using System.Collections.Generic;

namespace Shoppinglist.Data.Models;

public class Recipie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";

    public List<RecipieItem> RecipieItems { get; set; } = new();
}