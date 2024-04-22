using Menu.Models;
using Menu.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Menu.ViewModel;

public class FoodCategoryPageViewModel
{

    public ICommand ViewCategoryCommand { get; private set; }
    public ICommand UpdateCategoryCommand { get; private set; }
    public ICommand DeleteCategoryCommand { get; private set; }
    public ICommand AddCategoryCommand { get; private set; }

    private readonly FoodCategoryService _foodCategoryService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<FoodCategory> Categories { get; set; } = [];

    public FoodCategoryPageViewModel(FoodCategoryService foodCategoryService, INavigationService navigationService)
    {
        _foodCategoryService = foodCategoryService;
        _navigationService = navigationService;

        Task.Run(LoadCategoriesAsync);

        AddCategoryCommand = new Command(AddCategoryAsync);

        UpdateCategoryCommand = new Command<FoodCategory>(async (category) =>
        {
            if (!await _foodCategoryService.CategoryExistsAsync(category.CategoryName, category.Id) && category != null)
            {
                await _foodCategoryService.UpdateCategoryAsync(category);
            }
        });

        DeleteCategoryCommand = new Command<FoodCategory>(async (category) =>
        {
            await _foodCategoryService.DeleteCategoryAsync(category);
            Categories.Remove(category);
        });

        ViewCategoryCommand = new Command<FoodCategory>(async (category) =>
        {
            if (category != null)
            {
                await _navigationService.NavigateToFoodItemPage(category);
            }
        });
    }

    public async Task LoadCategoriesAsync()
    {
        var categoriesFromDb = await _foodCategoryService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categoriesFromDb)
        {
            Categories.Add(category);
        }
    }

    public async void AddCategoryAsync(object parameter)
    {
        var inputs = (Tuple<string, int>)parameter;
        string categoryName = inputs.Item1;
        int numberOfItems = inputs.Item2;
        var newCategory = new FoodCategory { CategoryName = categoryName, NumberOfItems = numberOfItems };
        await _foodCategoryService.InsertCategoryAsync(newCategory);
        Categories.Add(newCategory);
    }

    public Task<bool> CategoryExistsAsync(string categoryName)
    {
        return _foodCategoryService.CategoryExistsAsync(categoryName);
    }

}
