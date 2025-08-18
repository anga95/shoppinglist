using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using Shoppinglist.Data;

namespace shoppinglist;

public partial class App : Application
{
    public App(ShoppingListDbContext db)
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}