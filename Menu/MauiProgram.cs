using Menu.Service;
using Menu.ViewModel;
using Menu.Views;

namespace Menu;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddTransient<FoodCategoryService>();
        builder.Services.AddTransient<FoodItemService>();

        builder.Services.AddTransient<FoodCategoryPageViewModel>();
        builder.Services.AddTransient<FoodItemPageViewModel>();
        builder.Services.AddTransient<MainPageViewModel>();

        builder.Services.AddTransient<FoodCategoryPage>();
        builder.Services.AddTransient<FoodItemPage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }

}
