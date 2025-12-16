namespace ShoppingList.Models
{
    public class CategoryModel
    {
        public string Name { get; set; }
        public List<ProductModel> Products { get; set; }
        public List<CategoryModel> SubCategories { get; set; } = new();
        public bool IsExpanded { get; set; }

        public CategoryModel(string name)
        {
            Name = name;
            Products = new List<ProductModel>();
            IsExpanded = true;
        }
    }

}
