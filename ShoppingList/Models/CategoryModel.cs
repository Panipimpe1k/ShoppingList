namespace ShoppingList.Models
{
    public class CategoryModel
    {
        public string Name { get; set; }
        public List<ProductModel> Products { get; set; }
        public bool IsExpanded { get; set; }

        public CategoryModel(string name)
        {
            Name = name;
            Products = new List<ProductModel>();
            IsExpanded = true;
        }
    }

}
