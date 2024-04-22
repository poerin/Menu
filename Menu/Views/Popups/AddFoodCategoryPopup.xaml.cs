namespace Menu.Views.Popups;

public partial class AddFoodCategoryPopup : ContentView
{
    public event EventHandler? Accept;
    public event EventHandler? Cancel;

    public string CategoryName
    {
        get { return CategoryNameEntry.Text; }
    }

    public string NumberOfItems
    {
        get { return NumberOfItemsEntry.Text; }
    }

    public AddFoodCategoryPopup()
    {
        InitializeComponent();

        AcceptButton.Clicked += (sender, e) => { Accept?.Invoke(sender, e); };
        CancelButton.Clicked += (sender, e) => { Cancel?.Invoke(sender, e); };
    }
}