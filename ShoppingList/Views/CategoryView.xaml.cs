using ShoppingList.Models;
namespace ShoppingList.Views;

public partial class CategoryView : ContentView
{
    public CategoryModel Model { get; }

	private readonly Action _refreshUI;
	private readonly Action _save;
    public CategoryView(CategoryModel model, Action save, Action refreshUI)
	{
		InitializeComponent();
		Model = model;
		_refreshUI = refreshUI;
		_save = save;

		HeaderButton.Text = model.Name;
		RefreshProducts();
	}

	private void OnToggle(object sender, EventArgs e)
	{
		Model.IsExpanded = !Model.IsExpanded;
		ProductsContainer.IsVisible = Model.IsExpanded;
		_save();
	}

	public void RefreshProducts()
	{
		ProductsContainer.Children.Clear();
		foreach(var p in Model.Products)
		{
			ProductsContainer.Children.Add(new ProductView(p, _save, _refreshUI));
		}
	}
}