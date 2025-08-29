using System.Collections.Generic;

namespace Shoppinglist.Data.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = "";

    public List<RecipeItem> RecipeItems { get; set; } = new();
}