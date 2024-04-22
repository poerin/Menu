using Menu.Service;
using Menu.Views;

namespace Menu;

public partial class App : Application
{

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        var databaseService = serviceProvider.GetRequiredService<IDatabaseService>();
        databaseService.InitializeDatabase();
        MainPage mainPage = serviceProvider.GetRequiredService<MainPage>();
        var navigationService = serviceProvider.GetRequiredService<INavigationService>();
        navigationService.MainPage = mainPage;
        MainPage = new NavigationPage(mainPage);
    }

}
