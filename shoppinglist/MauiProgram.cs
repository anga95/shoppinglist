using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using shoppinglist.Components.Pages;
using Shoppinglist.Data;
using shoppinglist.Services;

namespace shoppinglist;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(f => f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        // SQLite-fil i appens data-katalog
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "shoppinglist.db");
        Console.WriteLine($"[DB] Path: {dbPath}");
        builder.Services.AddDbContext<ShoppingListDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<ShoppingListService>();
        builder.Services.AddScoped<HomeViewModel>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ShoppingListDbContext>();
            db.Database.Migrate();
        }

        return app;
    }
}