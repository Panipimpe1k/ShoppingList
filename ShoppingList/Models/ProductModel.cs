using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models
{
    public class ProductModel
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public bool IsBought { get; set; }

        public ProductModel(string name, string unit, int quantity = 1)
        {
            Name = name;
            Unit = unit;
            Quantity = quantity;
            IsBought = false;
        }
    }
}
