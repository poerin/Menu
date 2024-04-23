using Menu.ViewModel;
using Menu.Views.Popups;

namespace Menu.Views;

public partial class FoodCategoryPage : ContentPage
{
    private readonly FoodCategoryPageViewModel _viewModel;

    public FoodCategoryPage(FoodCategoryPageViewModel viewModel)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        ShowAddFoodCategoryPopup();
    }

    private void ShowAddFoodCategoryPopup()
    {
        var popup = new AddFoodCategoryPopup();
        popup.Accept += async (sender, e) =>
        {
            string categoryName = popup.CategoryName;
            string numberOfItemsText = popup.NumberOfItems;

            if (await _viewModel.CategoryExistsAsync(categoryName))
            {
                await DisplayAlert("错误", "已存在该类别名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                await DisplayAlert("错误", "请输入有效的类别名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(numberOfItemsText) || !int.TryParse(numberOfItemsText, out int numberOfItems) || numberOfItems < 0)
            {
                await DisplayAlert("错误", "请输入有效的食物数量。", "确认");
                return;
            }

            _viewModel.AddCategoryCommand.Execute(Tuple.Create(categoryName, numberOfItems));
            MainGrid.Children.Remove(popup);
        };
        popup.Cancel += (sender, e) =>
        {
            MainGrid.Children.Remove(popup);
        };

        MainGrid.Children.Add(popup);
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is FoodCategoryPageViewModel.FoodCategoryViewModel categoryVM)
        {
            bool confirmDelete = await DisplayAlert("提示", $"确定要删除{categoryVM.OriginalCategoryName}类别吗？", "确认", "取消");
            if (confirmDelete)
            {
                _viewModel.DeleteCategoryCommand.Execute(categoryVM);
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.UpdateStatusChanged += OnUpdateStatusChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.UpdateStatusChanged -= OnUpdateStatusChanged;
    }

    private async void OnUpdateStatusChanged(object? sender, bool e)
    {
        if (!e)
        {
            await DisplayAlert("错误", "已存在该类别名称。", "确认");
        }
    }

}