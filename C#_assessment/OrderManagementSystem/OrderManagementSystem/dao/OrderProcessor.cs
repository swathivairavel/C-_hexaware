using OrderManagementSystem.entity;
using OrderManagementSystem.exception;
using OrderManagementSystem.util;
using System.Data.SqlClient;


namespace OrderManagementSystem.dao
{
    public class OrderProcessor : IOrderManagementRepository
    {
        #region createUser
        public void CreateUser(User user)
        {
            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();
                    string query = @"INSERT INTO [user] (username, [password], [role])
                                     VALUES (@Username, @Password, @Role)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Role", user.Role);

                        cmd.ExecuteNonQuery();
                        Console.WriteLine("User created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
            }
        }
        #endregion

        #region createProduct
        public void CreateProduct(User user, Product product)
        {
            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();

                    // Step 1: Check if user is admin
                    string role = null;
                    string checkQuery = "SELECT role FROM [user] WHERE userid = @UserId";
                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.UserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = reader["role"].ToString();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
                    {
                        Console.WriteLine("Only admin can create products.");
                        return;
                    }

                    // Step 2: Insert product
                    string insertQuery = @"INSERT INTO product (productname, description, price, quantityinstock, type)
                                   VALUES (@Name, @Desc, @Price, @Qty, @Type)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", product.ProductName);
                        cmd.Parameters.AddWithValue("@Desc", product.Description);
                        cmd.Parameters.AddWithValue("@Price", product.Price);
                        cmd.Parameters.AddWithValue("@Qty", product.QuantityInStock);
                        cmd.Parameters.AddWithValue("@Type", product.Type);

                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Product created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
            }
        }
        #endregion

        #region createOrder
        public void CreateOrder(User user, List<Product> products)
        {
            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();
                    int userId = user.UserId;

                    // Check if user exists
                    string checkUserQuery = "SELECT COUNT(*) FROM [user] WHERE userid = @UserId";
                    using (SqlCommand cmd = new SqlCommand(checkUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int count = (int)cmd.ExecuteScalar();

                        if (count == 0)
                        {
                            // Insert user if not exists and retrieve new ID
                            string insertUserQuery = @"INSERT INTO [user] (username, [password], [role])
                                               OUTPUT INSERTED.userid
                                               VALUES (@Username, @Password, @Role)";
                            using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@Username", user.Username);
                                insertCmd.Parameters.AddWithValue("@Password", user.Password);
                                insertCmd.Parameters.AddWithValue("@Role", user.Role);

                                using (SqlDataReader reader = insertCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        userId = Convert.ToInt32(reader["userid"]);
                                    }
                                }
                            }
                        }
                    }

                    // Insert order and retrieve new order ID
                    string insertOrderQuery = @"INSERT INTO orders (userid, orderdate)
                                        OUTPUT INSERTED.orderid
                                        VALUES (@UserId, GETDATE())";
                    int orderId = 0;
                    using (SqlCommand cmd = new SqlCommand(insertOrderQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                orderId = Convert.ToInt32(reader["orderid"]);
                            }
                        }
                    }

                    // Insert order items
                    foreach (var product in products)
                    {
                        string insertItemQuery = @"INSERT INTO orderitem (orderid, productid, quantity, price)
                                           VALUES (@OrderId, @ProductId, 1, @Price)";
                        using (SqlCommand cmd = new SqlCommand(insertItemQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                            cmd.Parameters.AddWithValue("@Price", product.Price);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Order created successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
            }
        }
        #endregion

        #region cancelOrder
        public void CancelOrder(int userId, int orderId)
        {
            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();

                    // Step 1: Check if user exists
                    string userCheckQuery = "SELECT COUNT(*) AS UserCount FROM [user] WHERE userid = @UserId";
                    using (SqlCommand cmd = new SqlCommand(userCheckQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int userCount = (int)(cmd.ExecuteScalar() ?? 0);
                        if (userCount == 0)
                        {
                            throw new UserNotFoundException($"User with ID {userId} not found.");
                        }
                    }

                    // Step 2: Check if order exists for the user
                    string orderCheckQuery = "SELECT COUNT(*) AS OrderCount FROM orders WHERE orderid = @OrderId AND userid = @UserId";
                    using (SqlCommand cmd = new SqlCommand(orderCheckQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int orderCount = (int)(cmd.ExecuteScalar() ?? 0);
                        if (orderCount == 0)
                        {
                            throw new OrderNotFoundException($"Order with ID {orderId} not found for user ID {userId}.");
                        }
                    }

                    // Step 3: Delete related items from orderitem table
                    string deleteItemsQuery = "DELETE FROM orderitem WHERE orderid = @OrderId";
                    using (SqlCommand cmd = new SqlCommand(deleteItemsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    // Step 4: Delete the order itself
                    string deleteOrderQuery = "DELETE FROM orders WHERE orderid = @OrderId";
                    using (SqlCommand cmd = new SqlCommand(deleteOrderQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine($"Order {orderId} for user {userId} cancelled successfully.");
                }
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine($"User Error: {ex.Message}");
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine($"Order Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
        }
        #endregion

        #region getAllproduct
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();
                    string query = "SELECT * FROM product";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product p = new Product
                            {
                                ProductId = (int)reader["productid"],
                                ProductName = reader["productname"].ToString(),
                                Description = reader["description"].ToString(), // no null check
                                Price = (decimal)reader["price"],
                                QuantityInStock = (int)reader["quantityinstock"],
                                Type = reader["type"].ToString()
                            };

                            products.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }

            return products;
        }
        #endregion
        #region getOrderBYUser
        public List<Product> GetOrderByUser(User user)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (SqlConnection conn = DbConnUtil.GetConnectionObject())
                {
                    conn.Open();

                    string query = @"
                SELECT p.productid, p.productname, p.description, p.price, p.quantityinstock, p.type
                FROM product p
                JOIN order_item oi ON p.productid = oi.productid
                JOIN orders o ON oi.orderid = o.orderid
                WHERE o.userid = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.UserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product p = new Product
                                {
                                    ProductId = (int)reader["productid"],
                                    ProductName = reader["productname"].ToString(),
                                    Description = reader["description"].ToString(), // no null check
                                    Price = (decimal)reader["price"],
                                    QuantityInStock = (int)reader["quantityinstock"],
                                    Type = reader["type"].ToString()
                                };
                                products.Add(p);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user orders: {ex.Message}");
            }

            return products;
        }
        #endregion
    }
}

