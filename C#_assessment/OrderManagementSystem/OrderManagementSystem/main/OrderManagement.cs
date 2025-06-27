using OrderManagementSystem.util;
using OrderManagementSystem.dao;
using OrderManagementSystem.entity;

namespace OrderManagementSystem.main
{
    public class OrderManagementMain
    {
        public static void Run()
        {
            OrderProcessor processor = new OrderProcessor();

            while (true)
            {
                Console.WriteLine("\n--- Order Management Menu ---");
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. Create Product");
                Console.WriteLine("3. Cancel Order");
                Console.WriteLine("4. Get All Products");
                Console.WriteLine("5. Get Orders by User");
                Console.WriteLine("6. Exit");

                Console.Write("Enter your choice: ");
                string input = Console.ReadLine();
                int choice = Convert.ToInt32(input);

                switch (choice)
                {
                    case 1:
                        User newUser = new User();
                        Console.Write("Enter Username: ");
                        newUser.Username = Console.ReadLine();
                        Console.Write("Enter Password: ");
                        newUser.Password = Console.ReadLine();
                        Console.Write("Enter Role (Admin/User): ");
                        newUser.Role = Console.ReadLine();
                        processor.CreateUser(newUser);
                        break;

                    case 2:
                        User admin = new User();
                        Console.Write("Enter Admin User ID: ");
                        admin.UserId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Product Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter Description: ");
                        string desc = Console.ReadLine();
                        Console.Write("Enter Price: ");
                        decimal price = decimal.Parse(Console.ReadLine());
                        Console.Write("Enter Quantity in Stock: ");
                        int qty = int.Parse(Console.ReadLine());
                        Console.Write("Enter Type (Electronics/Clothing): ");
                        string type = Console.ReadLine();

                        Product product = new Product
                        {
                            ProductName = name,
                            Description = desc,
                            Price = price,
                            QuantityInStock = qty,
                            Type = type
                        };

                        processor.CreateProduct(admin, product);
                        break;

                    case 3:
                        Console.Write("Enter User ID: ");
                        int userId = int.Parse(Console.ReadLine());
                        Console.Write("Enter Order ID: ");
                        int orderId = int.Parse(Console.ReadLine());
                        processor.CancelOrder(userId, orderId);
                        break;

                    case 4:
                        List<Product> products = processor.GetAllProducts();
                        Console.WriteLine("\n--- All Products ---");
                        foreach (var p in products)
                        {
                            Console.WriteLine($"{p.ProductId}: {p.ProductName} | {p.Description} | Rs. {p.Price} | Stock: {p.QuantityInStock} | Type: {p.Type}");
                        }
                        break;

                    case 5:
                        Console.Write("Enter User ID: ");
                        int uid = int.Parse(Console.ReadLine());
                        User user = new User { UserId = uid };
                        List<Product> userProducts = processor.GetOrderByUser(user);
                        Console.WriteLine("\n--- Products Ordered by User ---");
                        foreach (var p in userProducts)
                        {
                            Console.WriteLine($"{p.ProductId}: {p.ProductName} | {p.Description} | Rs. {p.Price} | Type: {p.Type}");
                        }
                        break;

                    case 6:
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}
