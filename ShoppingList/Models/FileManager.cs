using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
