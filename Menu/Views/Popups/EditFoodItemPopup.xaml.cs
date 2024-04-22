using Menu.Models;

namespace Menu.Views.Popups;

public partial class EditFoodItemPopup : ContentView
{

    public event EventHandler? Accept;
    public event EventHandler? Cancel;
    public event EventHandler? Delete;

    public string FoodName
    {
        get { return FoodNameEntry.Text; }
    }

    public string Weight
    {
        get { return WeightEntry.Text; }
    }

    public string GroupNumber
    {
        get { return GroupNumberEntry.Text; }
    }

    public EditFoodItemPopup(FoodItem item)
    {
        InitializeComponent();
        FoodNameEntry.Text = item.FoodName;
        WeightEntry.Text = item.Weight.ToString();
        GroupNumberEntry.Text = item.GroupNumber.ToString();

        AcceptButton.Clicked += (sender, e) => { Accept?.Invoke(sender, e); };
        CancelButton.Clicked += (sender, e) => { Cancel?.Invoke(sender, e); };
        DeleteButton.Clicked += (sender, e) => { Delete?.Invoke(sender, e); };
    }
}