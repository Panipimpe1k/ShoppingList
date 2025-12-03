using ShoppingList.Models;

namespace ShoppingList.Views;

public partial class MainPage : ContentPage
{
    private readonly List<ProductModel> _products = new();

    public MainPage()
    {
        InitializeComponent();
        LoadProducts();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = string.IsNullOrWhiteSpace(ProductNameEntry.Text) ? "Unnamed" : ProductNameEntry.Text;

        var unit = UnitPicker.SelectedItem?.ToString() ?? "pcs";

        int quantity;
        if (!int.TryParse(QuantityEntry.Text, out quantity) || quantity <= 0)
        {
            quantity = 1;
            await DisplayAlert("Warning", "Invalid quantity. Replaced with 1", "OK");
        }

        var model = new ProductModel(name, unit, quantity);
        _products.Add(model);

        var view = new ProductView(model, SaveProducts);
        ProductContainer.Children.Add(view);

        ProductNameEntry.Text = string.Empty;
        UnitPicker.SelectedIndex = -1;
        QuantityEntry.Text = string.Empty;

        SaveProducts();
    }


    private void SaveProducts()
    {
        FileManager.Save(_products);
    }

    private void LoadProducts()
    {
        var loaded = FileManager.Load();
        _products.Clear();
        _products.AddRange(loaded);

        ProductContainer.Children.Clear();
        foreach (var model in _products.OrderBy(p => p.IsBought))
        {
            ProductContainer.Children.Add(new ProductView(model, SaveProducts));
        }
    }
}
