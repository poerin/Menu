using Menu.Models;
using SQLite;

namespace Menu.Service;

public class FoodCategoryService(IDatabaseService databaseService)
{
    private readonly SQLiteAsyncConnection _database = databaseService.GetConnection();

    public Task<int> InsertCategoryAsync(FoodCategory category)
    {
        return _database.InsertAsync(category);
    }

    public Task<List<FoodCategory>> GetCategoriesAsync()
    {
        return _database.Table<FoodCategory>().ToListAsync();
    }

    public Task<FoodCategory> GetCategoryByIDAsync(int id)
    {
        return _database.Table<FoodCategory>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    public Task<int> UpdateCategoryAsync(FoodCategory category)
    {
        return _database.UpdateAsync(category);
    }

    public Task<int> DeleteCategoryAsync(FoodCategory category)
    {
        return _database.DeleteAsync(category);
    }

    public async Task<int> DeleteCategoryAndItemsAsync(FoodCategory category)
    {
        foreach (var item in await _database.Table<FoodItem>().Where(i => i.FoodCategoryId == category.Id).ToListAsync())
        {
            await _database.DeleteAsync(item);
        }
        return await _database.DeleteAsync(category);
    }

    public async Task<bool> CategoryExistsAsync(string categoryName, int Id = 0)
    {
        FoodCategory category = await _database.Table<FoodCategory>()
                                     .Where(c => c.CategoryName == categoryName && c.Id != Id)
                                     .FirstOrDefaultAsync();
        return category != null;
    }

}
