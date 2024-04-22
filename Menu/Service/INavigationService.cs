using Menu.Models;
using Menu.Views;

namespace Menu.Service;

public interface INavigationService
{
    MainPage? MainPage { get; set; }

    Task NavigateToFoodCategoryPage();
    Task NavigateToFoodItemPage(FoodCategory category);
}
