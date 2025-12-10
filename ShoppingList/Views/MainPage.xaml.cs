using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ShoppingList.Models;

namespace ShoppingList.Views;

public partial class MainPage : ContentPage
{
    private readonly List<CategoryModel> _categories = new();
    private readonly List<string> _stores = new() { "All", "Biedronka", "Lidl", "Kaufland", "Selgros", "Auchan" };
    private readonly List<string> _units = new() { "pcs", "kg", "g", "l", "ml" };
    private bool _shoppingMode = false;

    public MainPage()
    {
        InitializeComponent();
        UnitPicker.ItemsSource = _units;
        StorePicker.ItemsSource = _stores.Where(s => s != "All").ToList();
        StoreFilterPicker.ItemsSource = _stores;
        StoreFilterPicker.SelectedIndex = 0;
        StoreFilterPicker.SelectedIndexChanged += OnStoreFilterChanged;
        ToggleAddPanelButton.Text = "Show add product";
        AddPanelFrame.IsVisible = false;
        LoadCategoriesAndInitUI();
    }

    private void LoadCategoriesAndInitUI()
    {
        var loaded = FileManager.LoadCategories();
        if (loaded == null || loaded.Count == 0)
        {
            _categories.Clear();
            _categories.Add(new CategoryModel("Dairy"));
            _categories.Add(new CategoryModel("Vegetables"));
            _categories.Add(new CategoryModel("Electronics"));
            _categories.Add(new CategoryModel("Home Appliances"));
            SaveCategories();
        }
        else
        {
            _categories.Clear();
            _categories.AddRange(loaded);
        }
        RefreshCategoryPicker();
        RefreshUI();
    }

    private void SaveCategories()
    {
        FileManager.SaveCategories(_categories);
    }

    private void RefreshCategoryPicker()
    {
        CategoryPicker.ItemsSource = _categories.Select(c => c.Name).ToList();
        if (CategoryPicker.ItemsSource.Cast<string>().Any())
            CategoryPicker.SelectedIndex = 0;
    }

    private void RefreshUI()
    {
        MainContainer.Children.Clear();
        if (_shoppingMode)
            DisplayShoppingList();
        else
            DisplayCategoryList();
    }

    private void DisplayCategoryList()
    {
        foreach (var category in _categories)
        {
            var catView = new CategoryView(category, SaveCategories, RefreshUI);
            MainContainer.Children.Add(catView);
        }
    }

    private void DisplayShoppingList()
    {
        string selectedStore = StoreFilterPicker.SelectedItem?.ToString() ?? "All";
        var items = _categories
            .SelectMany(cat => cat.Products.Select(p => new { Category = cat.Name, Product = p }))
            .Where(x => !x.Product.IsBought)
            .Where(x => selectedStore == "All" || x.Product.Store == selectedStore)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Product.Name)
            .ToList();
        string lastCategory = null;
        foreach (var item in items)
        {
            if (lastCategory != item.Category)
            {
                lastCategory = item.Category;
                var header = new Label
                {
                    Text = item.Category,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                MainContainer.Children.Add(header);
            }
            var pv = new ProductView(item.Product, SaveCategories, RefreshUI);
            MainContainer.Children.Add(pv);
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = string.IsNullOrWhiteSpace(ProductNameEntry.Text) ? "Unnamed" : ProductNameEntry.Text.Trim();
        var unit = UnitPicker.SelectedItem?.ToString() ?? "pcs";
        var store = StorePicker.SelectedItem?.ToString() ?? _stores[1];
        if (!int.TryParse(QuantityEntry.Text, out int quantity) || quantity <= 0)
        {
            quantity = 1;
            await DisplayAlert("Warning", "Invalid quantity. Replaced with 1", "OK");
        }
        if (CategoryPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Warning", "Please select a category.", "OK");
            return;
        }
        var categoryName = CategoryPicker.SelectedItem.ToString();
        var category = _categories.FirstOrDefault(c => c.Name == categoryName);
        if (category == null)
        {
            await DisplayAlert("Error", "Selected category not found.", "OK");
            return;
        }
        var product = new ProductModel(name, unit, quantity, store);
        category.Products.Add(product);
        SaveCategories();
        RefreshUI();
        ProductNameEntry.Text = string.Empty;
        QuantityEntry.Text = string.Empty;
        UnitPicker.SelectedIndex = -1;
        StorePicker.SelectedIndex = -1;
        CategoryPicker.SelectedIndex = -1;
        AddPanelFrame.IsVisible = false;
        ToggleAddPanelButton.Text = "Show add product";
    }

    private void OnToggleAddPanelClicked(object sender, EventArgs e)
    {
        bool isVisible = AddPanelFrame.IsVisible;
        AddPanelFrame.IsVisible = !isVisible;
        ToggleAddPanelButton.Text = isVisible ? "Show add product" : "Hide add product";
    }

    private void OnCancelAddClicked(object sender, EventArgs e)
    {
        ProductNameEntry.Text = string.Empty;
        QuantityEntry.Text = string.Empty;
        UnitPicker.SelectedIndex = -1;
        StorePicker.SelectedIndex = -1;
        CategoryPicker.SelectedIndex = -1;
        AddPanelFrame.IsVisible = false;
        ToggleAddPanelButton.Text = "Show add product";
    }

    private void OnCategoriesModeClicked(object sender, EventArgs e)
    {
        _shoppingMode = false;
        StoreFilterPicker.IsVisible = false;
        RefreshUI();
    }

    private void OnShoppingModeClicked(object sender, EventArgs e)
    {
        _shoppingMode = true;
        StoreFilterPicker.IsVisible = true;
        RefreshUI();
    }

    private void OnStoreFilterChanged(object sender, EventArgs e)
    {
        if (_shoppingMode)
            RefreshUI();
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("New category", "Enter category name:");
        if (string.IsNullOrWhiteSpace(result)) return;
        var name = result.Trim();
        if (_categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            await DisplayAlert("Info", "Category already exists.", "OK");
            return;
        }
        _categories.Add(new CategoryModel(name));
        SaveCategories();
        RefreshCategoryPicker();
        RefreshUI();
    }

    private async void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Info", "No category selected.", "OK");
            return;
        }
        var name = CategoryPicker.SelectedItem.ToString();
        var cat = _categories.FirstOrDefault(c => c.Name == name);
        if (cat == null) return;
        bool ok = await DisplayAlert("Confirm", $"Delete category '{name}' and all its products?", "Yes", "No");
        if (!ok) return;
        _categories.Remove(cat);
        SaveCategories();
        RefreshCategoryPicker();
        RefreshUI();
    }

    public void ForceRefresh() => RefreshUI();
}
