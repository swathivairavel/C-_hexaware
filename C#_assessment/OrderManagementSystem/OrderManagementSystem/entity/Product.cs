﻿

namespace OrderManagementSystem.entity
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string Type { get; set; } 

        public Product() { }

        public Product(int productId, string productName, string description, decimal price, int quantityInStock, string type)
        {
            ProductId = productId;
            ProductName = productName;
            Description = description;
            Price = price;
            QuantityInStock = quantityInStock;
            Type = type;
        }
    }
}


