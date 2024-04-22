using Menu.ViewModel;
using Menu.Views.Popups;

namespace Menu.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        await _viewModel.InitializeAsync();
        base.OnAppearing();
    }

    private void OnConfigClicked(object sender, EventArgs e)
    {
        var popup = new ConfigPopup();
        popup.ImportData += async (object? sender, EventArgs e) =>
        {
            if (await _viewModel.ImportData())
            {
                await _viewModel.InitializeAsync();
                await DisplayAlert("成功", "数据导入完成。", "确认");
            }
            else
            {
                await DisplayAlert("错误", "剪贴板中不存在有效数据。", "确认");
            }
        };
        popup.ExportData += async (object? sender, EventArgs e) =>
        {
            if (await _viewModel.ExportData())
            {
                await DisplayAlert("成功", "数据已导出到剪贴板。", "确认");
            }
            else
            {
                await DisplayAlert("错误", "写入剪贴板失败。", "确认");
            }
        };

        MainGrid.Children.Add(popup);
    }

}
