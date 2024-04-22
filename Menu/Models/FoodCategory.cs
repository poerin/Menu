using SQLite;

namespace Menu.Models;

public class FoodCategory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public int NumberOfItems { get; set; }
}
