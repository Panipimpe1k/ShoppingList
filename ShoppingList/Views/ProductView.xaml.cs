using Microsoft.Maui.Controls;
using ShoppingList.Models;

namespace ShoppingList.Views;

public partial class ProductView : ContentView
{
	private readonly ProductModel _model;
	private readonly Action _onChanged;
	public ProductView(ProductModel model, Action onChanged)
	{
		InitializeComponent();
		_model = model;
		_onChanged = onChanged;

		NameLabel.Text = model.Name;
		UnitLabel.Text = $"Unit: {model.Unit}";
		QuantityEntry.Text = model.Quantity.ToString();

		UpdateVisualState();
	}

	private void OnPlusClicked(object sender, EventArgs e)
	{
		_model.Quantity++;
		QuantityEntry.Text = _model.Quantity.ToString();
		_onChanged?.Invoke();
	}

    private void OnMinusClicked(object sender, EventArgs e)
    {
		if (_model.Quantity > 0)
			_model.Quantity--;
        QuantityEntry.Text = _model.Quantity.ToString();
        _onChanged?.Invoke();
    }

    private void OnBoughtClicked(object sender, EventArgs e)
    {
		_model.IsBought = !_model.IsBought;
		UpdateVisualState();
		_onChanged?.Invoke();
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if(Parent is Layout parent)
		{
			parent.Children.Remove(this);
			_onChanged?.Invoke();
		}
    }

	private void UpdateVisualState()
	{
		if (_model.IsBought)
		{
			NameLabel.TextDecorations = TextDecorations.Strikethrough;
			Opacity = 0.5;
		}
		else
		{
			NameLabel.TextDecorations = TextDecorations.None;
			Opacity = 1;
		}
	}
}