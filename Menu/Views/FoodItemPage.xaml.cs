using Menu.Models;
using Menu.ViewModel;
using Menu.Views.Popups;

namespace Menu.Views;

public partial class FoodItemPage : ContentPage
{
    private readonly FoodItemPageViewModel _viewModel;

    public FoodItemPage(FoodItemPageViewModel viewModel)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _viewModel = viewModel;
        BindingContext = viewModel;
        LoadItems();
    }

    private async void LoadItems()
    {
        await _viewModel.LoadFoodItemsAsync();
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        ShowAddFoodItemPopup();
    }

    private void OnFoodNameClicked(object sender, EventArgs e)
    {
        var selectedFoodItem = (FoodItem)((Button)sender).BindingContext;
        ShowEditFoodItemPopup(selectedFoodItem);
    }

    private void OnFoodWeightClicked(object sender, EventArgs e)
    {
        var selectedFoodItem = (FoodItem)((Button)sender).BindingContext;
        ShowEditFoodItemPopup(selectedFoodItem);
    }

    private void ShowAddFoodItemPopup()
    {
        var popup = new AddFoodItemPopup();
        popup.Accept += async (sender, e) =>
        {
            string foodName = popup.FoodName;
            string weightText = popup.Weight;
            string groupNumberText = popup.GroupNumber;

            if (await _viewModel.ItemExistsAsync(foodName))
            {
                await DisplayAlert("错误", "该类别中已存在该食品名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(foodName))
            {
                await DisplayAlert("错误", "请输入有效的食品名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(weightText) || !int.TryParse(weightText, out int weight) || weight < 0)
            {
                await DisplayAlert("错误", "请输入有效的权重。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(groupNumberText))
            {
                groupNumberText = "0";
            }

            if (!int.TryParse(groupNumberText, out int groupNumber) || groupNumber < 0)
            {
                await DisplayAlert("错误", "请输入有效的组号。", "确认");
                return;
            }

            await _viewModel.AddItemAsync(foodName, weight, groupNumber);
            MainGrid.Children.Remove(popup);
        };
        popup.Cancel += (sender, e) =>
        {
            MainGrid.Children.Remove(popup);
        };

        MainGrid.Children.Add(popup);
    }

    private void ShowEditFoodItemPopup(FoodItem item)
    {
        var popup = new EditFoodItemPopup(item);
        popup.Accept += async (sender, e) =>
        {
            string foodName = popup.FoodName;
            string weightText = popup.Weight;
            string groupNumberText = popup.GroupNumber;

            if (item.FoodName != foodName && await _viewModel.ItemExistsAsync(foodName))
            {
                await DisplayAlert("错误", "该类别中已存在该食品名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(foodName))
            {
                await DisplayAlert("错误", "请输入有效的食品名称。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(weightText) || !int.TryParse(weightText, out int weight) || weight < 0)
            {
                await DisplayAlert("错误", "请输入有效的权重。", "确认");
                return;
            }

            if (string.IsNullOrEmpty(groupNumberText))
            {
                groupNumberText = "0";
            }

            if (!int.TryParse(groupNumberText, out int groupNumber) || groupNumber < 0)
            {
                await DisplayAlert("错误", "请输入有效的组号。", "确认");
                return;
            }

            item.FoodName = foodName;
            item.Weight = weight;
            item.GroupNumber = groupNumber;
            await _viewModel.UpdateItemAsync(item);
            MainGrid.Children.Remove(popup);
        };
        popup.Delete += async (sender, e) =>
        {
            await _viewModel.DeleteItemAsync(item);
            MainGrid.Children.Remove(popup);
        };
        popup.Cancel += (sender, e) =>
        {
            MainGrid.Children.Remove(popup);
        };

        MainGrid.Children.Add(popup);
    }

}