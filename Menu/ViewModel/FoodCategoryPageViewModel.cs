using Menu.Models;
using Menu.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Menu.ViewModel;

public class FoodCategoryPageViewModel
{
    public class FoodCategoryViewModel(FoodCategory category) : INotifyPropertyChanged
    {
        private string _originalCategoryName = category.CategoryName;
        private int _originalNumberOfItems = category.NumberOfItems;

        public FoodCategory Category => category;
        public bool IsCategoryNameModified => category.CategoryName != OriginalCategoryName;
        public bool IsNumberOfItemsModified => category.NumberOfItems != OriginalNumberOfItems;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string OriginalCategoryName
        {
            get => _originalCategoryName;
            set
            {
                if (_originalCategoryName != value)
                {
                    _originalCategoryName = value;
                    OnPropertyChanged(nameof(OriginalCategoryName));
                    OnPropertyChanged(nameof(IsCategoryNameModified));
                }
            }
        }

        public int OriginalNumberOfItems
        {
            get => _originalNumberOfItems;
            set
            {
                if (_originalNumberOfItems != value)
                {
                    _originalNumberOfItems = value;
                    OnPropertyChanged(nameof(OriginalNumberOfItems));
                    OnPropertyChanged(nameof(IsNumberOfItemsModified));
                }
            }
        }

        public string CategoryName
        {
            get => category.CategoryName;
            set
            {
                if (category.CategoryName != value)
                {
                    category.CategoryName = value;
                    OnPropertyChanged(nameof(CategoryName));
                    OnPropertyChanged(nameof(IsCategoryNameModified));
                }
            }
        }

        public int NumberOfItems
        {
            get => category.NumberOfItems;
            set
            {
                if (category.NumberOfItems != value)
                {
                    category.NumberOfItems = value;
                    OnPropertyChanged(nameof(NumberOfItems));
                    OnPropertyChanged(nameof(IsNumberOfItemsModified));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public ICommand ViewCategoryCommand { get; private set; }
    public ICommand UpdateCategoryCommand { get; private set; }
    public ICommand DeleteCategoryCommand { get; private set; }
    public ICommand AddCategoryCommand { get; private set; }

    public event EventHandler<bool>? UpdateStatusChanged;

    private readonly FoodCategoryService _foodCategoryService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<FoodCategoryViewModel> Categories { get; set; } = [];

    public FoodCategoryPageViewModel(FoodCategoryService foodCategoryService, INavigationService navigationService)
    {
        _foodCategoryService = foodCategoryService;
        _navigationService = navigationService;

        Task.Run(LoadCategoriesAsync);

        AddCategoryCommand = new Command(AddCategoryAsync);

        UpdateCategoryCommand = new Command<FoodCategoryViewModel>(async (categoryVM) =>
        {
            var category = categoryVM.Category;
            if (!await _foodCategoryService.CategoryExistsAsync(category.CategoryName, category.Id) && category != null)
            {
                var result = await _foodCategoryService.UpdateCategoryAsync(category);
                if (result > 0)
                {
                    categoryVM.OriginalCategoryName = categoryVM.CategoryName;
                    categoryVM.OriginalNumberOfItems = categoryVM.NumberOfItems;
                    UpdateStatusChanged?.Invoke(this, true);
                }
            }
            else
            {
                UpdateStatusChanged?.Invoke(this, false);
            }
        });

        DeleteCategoryCommand = new Command<FoodCategoryViewModel>(async (categoryVM) =>
        {
            var category = categoryVM.Category;
            await _foodCategoryService.DeleteCategoryAsync(category);
            Categories.Remove(categoryVM);
        });

        ViewCategoryCommand = new Command<FoodCategoryViewModel>(async (categoryVM) =>
        {
            var category = categoryVM.Category;
            await _navigationService.NavigateToFoodItemPage(new FoodCategory { Id = category.Id, CategoryName = categoryVM.OriginalCategoryName, NumberOfItems = categoryVM.OriginalNumberOfItems });
        });
    }

    public async Task LoadCategoriesAsync()
    {
        var categoriesFromDb = await _foodCategoryService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categoriesFromDb)
        {
            Categories.Add(new FoodCategoryViewModel(category));
        }
    }

    public async void AddCategoryAsync(object parameter)
    {
        var inputs = (Tuple<string, int>)parameter;
        string categoryName = inputs.Item1;
        int numberOfItems = inputs.Item2;
        var newCategory = new FoodCategory { CategoryName = categoryName, NumberOfItems = numberOfItems };
        await _foodCategoryService.InsertCategoryAsync(newCategory);
        Categories.Add(new FoodCategoryViewModel(newCategory));
    }

    public Task<bool> CategoryExistsAsync(string categoryName)
    {
        return _foodCategoryService.CategoryExistsAsync(categoryName);
    }

}
