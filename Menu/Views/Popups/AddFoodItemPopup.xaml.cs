namespace Menu.Views.Popups;

public partial class AddFoodItemPopup : ContentView
{
    public event EventHandler? Accept;
    public event EventHandler? Cancel;

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

    public AddFoodItemPopup()
    {
        InitializeComponent();

        AcceptButton.Clicked += (sender, e) => { Accept?.Invoke(sender, e); };
        CancelButton.Clicked += (sender, e) => { Cancel?.Invoke(sender, e); };
    }
}