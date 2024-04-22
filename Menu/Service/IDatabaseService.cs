using SQLite;

namespace Menu.Service;

public interface IDatabaseService
{
    public string DatabasePath { get; }
    SQLiteAsyncConnection GetConnection();
    Task InitializeDatabase();
    void OpenConnection();
    Task CloseConnectionAsync();
}
