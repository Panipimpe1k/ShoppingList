using System.Text.Json;

namespace ShoppingList.Models
{
    public static class FileManager
    {
        private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "shoppinglist.json");

        public static void Save(List<ProductModel> products)
        {
            var json = JsonSerializer.Serialize(products);
            File.WriteAllText(FilePath, json);
        }

        public static List<ProductModel> Load()
        {
            if (!File.Exists(FilePath))
                return new List<ProductModel>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<ProductModel>>(json) ?? new();
        }

        private static readonly string CategoryPath =
        Path.Combine(FileSystem.AppDataDirectory, "categories.json");

        public static void SaveCategories(List<CategoryModel> categories)
        {
            var json = JsonSerializer.Serialize(categories);
            File.WriteAllText(CategoryPath, json);
        }

        public static List<CategoryModel> LoadCategories()
        {
            if (!File.Exists(CategoryPath))
                return new List<CategoryModel>();

            var json = File.ReadAllText(CategoryPath);
            return JsonSerializer.Deserialize<List<CategoryModel>>(json) ?? new();
        }
    }
}
