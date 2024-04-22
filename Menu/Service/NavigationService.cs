using Menu.Models;
using Menu.ViewModel;
using Menu.Views;

namespace Menu.Service;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{

    public MainPage? MainPage { get; set; }

    public async Task NavigateToFoodCategoryPage()
    {
        if (MainPage != null)
        {
            var page = serviceProvider.GetRequiredService<FoodCategoryPage>();
            await MainPage.Navigation.PushAsync(page);
        }
    }

    public async Task NavigateToFoodItemPage(FoodCategory category)
    {
        if (MainPage != null)
        {
            var viewModel = serviceProvider.GetRequiredService<FoodItemPageViewModel>();
            viewModel.FoodCategory = category;
            await MainPage.Navigation.PushAsync(new FoodItemPage(viewModel));
        }
    }
}
