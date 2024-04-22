using Menu.Models;
using Menu.Service;
using System.Collections.ObjectModel;

namespace Menu.ViewModel;

public class FoodItemPageViewModel(FoodItemService foodItemService)
{

    public class FoodGroupViewModel
    {
        public string GroupTitle { get; set; }
        public ObservableCollection<FoodItem> Foods { get; set; }

        public FoodGroupViewModel(int groupKey, IEnumerable<FoodItem> items)
        {
            if (groupKey == 0)
                GroupTitle = $"未分组";
            else
                GroupTitle = $"第 {groupKey} 组";
            Foods = new ObservableCollection<FoodItem>(items);
        }
    }

    public FoodCategory? FoodCategory { get; set; }

    public ObservableCollection<FoodGroupViewModel> FoodGroups { get; set; } = [];

    public async Task LoadFoodItemsAsync()
    {
        var foodItems = await foodItemService.GetItemsByCategoryIdAsync(FoodCategory.Id);

        var groupedFoodItems = foodItems
            .OrderBy(item => item.GroupNumber)
            .GroupBy(item => item.GroupNumber)
            .Select(g => new FoodGroupViewModel(g.Key, g))
            .ToList();

        FoodGroups.Clear();
        foreach (var groupViewModel in groupedFoodItems)
        {
            FoodGroups.Add(groupViewModel);
        }
    }

    public async Task AddItemAsync(string foodName, int weight, int groupNumber)
    {
        var newItem = new FoodItem { FoodCategoryId = FoodCategory.Id, FoodName = foodName, Weight = weight, GroupNumber = groupNumber };
        await foodItemService.InsertItemAsync(newItem);
        await LoadFoodItemsAsync();
    }

    public async Task UpdateItemAsync(FoodItem item)
    {
        await foodItemService.UpdateItemAsync(item);
        await LoadFoodItemsAsync();
    }

    public async Task DeleteItemAsync(FoodItem item)
    {
        await foodItemService.DeleteItemAsync(item);
        await LoadFoodItemsAsync();
    }

    public Task<bool> ItemExistsAsync(string foodName)
    {
        return foodItemService.ItemExistsAsync(FoodCategory.Id, foodName);
    }

}
