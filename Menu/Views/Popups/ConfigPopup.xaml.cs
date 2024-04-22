namespace Menu.Views.Popups;

public partial class ConfigPopup : ContentView
{
    public event EventHandler? ImportData;
    public event EventHandler? ExportData;

    public ConfigPopup()
    {
        InitializeComponent();

        ImportDataButton.Clicked += (sender, e) => { ImportData?.Invoke(sender, e); IsVisible = false; };
        ExportDataButton.Clicked += (sender, e) => { ExportData?.Invoke(sender, e); IsVisible = false; };
    }

    private void OnBackgroundTapped(object sender, EventArgs e)
    {
        IsVisible = false;
    }

}