using SQLite;

namespace Menu.Models;

public class FoodItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int FoodCategoryId { get; set; }
    public string FoodName { get; set; }
    public decimal Weight { get; set; }
    public int GroupNumber { get; set; }
}
