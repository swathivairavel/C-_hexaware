

namespace OrderManagementSystem.entity
{
    public class Electronics : Product
    {
        public string Brand { get; set; }
        public int WarrantyPeriod { get; set; } 

        public Electronics() { }

        public Electronics(int productId, string productName, string description, decimal price, int quantityInStock, string type,
                           string brand, int warrantyPeriod)
            : base(productId, productName, description, price, quantityInStock, type)
        {
            Brand = brand;

            WarrantyPeriod = warrantyPeriod;
        }
        
    }
}
