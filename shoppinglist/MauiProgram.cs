using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shoppinglist.Data;

namespace shoppinglist;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "shoppinglist.db");
        builder.Services.AddDbContext<ShoppingListDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        
        builder.Services.AddScoped<ShoppingListDbContext>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        
        var app = builder.Build();
        
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShoppingListDbContext>();
        db.Database.EnsureCreated();
        
        return builder.Build();
    }
}