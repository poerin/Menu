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
                await DisplayAlert("����", "�Ѵ��ڸ�������ơ�", "ȷ��");
                return;
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                await DisplayAlert("����", "��������Ч��������ơ�", "ȷ��");
                return;
            }

            if (string.IsNullOrEmpty(numberOfItemsText) || !int.TryParse(numberOfItemsText, out int numberOfItems) || numberOfItems < 0)
            {
                await DisplayAlert("����", "��������Ч��ʳ��������", "ȷ��");
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

}