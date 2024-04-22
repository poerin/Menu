using Menu.Models;
using SQLite;

namespace Menu.Service;

public class FoodItemService(IDatabaseService databaseService)
{
    private readonly SQLiteAsyncConnection _database = databaseService.GetConnection();

    public Task<int> InsertItemAsync(FoodItem item)
    {
        return _database.InsertAsync(item);
    }

    public Task<List<FoodItem>> GetItemsAsync()
    {
        return _database.Table<FoodItem>().ToListAsync();
    }

    public Task<FoodItem> GetItemByIDAsync(int id)
    {
        return _database.Table<FoodItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public Task<int> UpdateItemAsync(FoodItem item)
    {
        return _database.UpdateAsync(item);
    }

    public Task<int> DeleteItemAsync(FoodItem item)
    {
        return _database.DeleteAsync(item);
    }

    public Task<List<FoodItem>> GetItemsByCategoryIdAsync(int categoryId)
    {
        return _database.Table<FoodItem>().Where(item => item.FoodCategoryId == categoryId).ToListAsync();
    }

    public async Task<bool> ItemExistsAsync(int foodCategoryId, string foodName)
    {
        var foodItem = await _database.Table<FoodItem>()
                                       .Where(item => item.FoodCategoryId == foodCategoryId && item.FoodName == foodName)
                                       .FirstOrDefaultAsync();
        return foodItem != null;
    }
}
