using Microsoft.EntityFrameworkCore;
using shoppinglist.Data;

namespace shoppinglist;

public partial class App : Application
{
    public App(ShoppingListDbContext db)
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}