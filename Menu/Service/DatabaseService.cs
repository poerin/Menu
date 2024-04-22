using Menu.Models;
using SQLite;

namespace Menu.Service;

public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection _database;
    public string DatabasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foods.db");

    public DatabaseService()
    {
        _database = new SQLiteAsyncConnection(DatabasePath);
    }

    public SQLiteAsyncConnection GetConnection()
    {
        return _database;
    }

    public async Task InitializeDatabase()
    {
        await _database.CreateTableAsync<FoodCategory>();
        await _database.CreateTableAsync<FoodItem>();
    }

    public void OpenConnection()
    {
        _database = new SQLiteAsyncConnection(DatabasePath);
    }

    public Task CloseConnectionAsync()
    {
        return _database.CloseAsync();
    }

}
