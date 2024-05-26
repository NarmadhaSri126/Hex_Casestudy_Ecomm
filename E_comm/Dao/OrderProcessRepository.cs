using System;
using E_comm.entity;
using E_comm.DBUtil;
using E_comm.exception;
using E_comm.Main;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace E_comm.dao
{
    public interface OrderProcessRepository
    {
        bool CreateProduct(Product product);

        bool CreateCustomer(Customer customer);

        bool DeleteProduct(string productId);

        bool DeleteCustomer(string customerId);

        bool AddToCart(Cart cart);

        bool RemoveFromCart(int customerId, int productId);

        List<Product> GetAllFromCart(Customer customer);

        bool PlaceOrder(Order order);

        List<Order> GetOrdersByCustomer(int customerId);
    }

    public class OrderProcessRepositoryImpl : OrderProcessRepository
    {
        //public string connstring = "Data Source=LAPTOP-8NH5PHBS;Initial Catalog=Ecommerce;Integrated Security=True";
        public bool CreateCustomer(Customer customer)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = "INSERT INTO customers (Name, Email, Password) " +
                                   "VALUES (@Name, @Email, @Password)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // command.Parameters.AddWithValue("@customer_id", customer.CustomerId);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Password", customer.Password);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return false;
            }
        }

        public bool CreateProduct(Product product)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    connection.Open();

                    // Check if a product with the same name and description already exists
                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE LOWER(Name) = LOWER(@Name) AND LOWER(Description) = LOWER(@Description) and price=@price";

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Name", product.Name);
                        checkCommand.Parameters.AddWithValue("@Description", product.Description);
                        checkCommand.Parameters.AddWithValue("@price", product.Price);
                        int existingProductsCount = (int)checkCommand.ExecuteScalar();

                        if (existingProductsCount > 0)
                        {
                            // Product already exists, update stock quantity
                            string updateQuery = "UPDATE Products SET StockQuantity = StockQuantity + @Quantity WHERE Name = @Name AND Description = @Description";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Name", product.Name);
                                updateCommand.Parameters.AddWithValue("@Description", product.Description);
                                updateCommand.Parameters.AddWithValue("@Quantity", product.StockQuantity);

                                int rowsAffected = updateCommand.ExecuteNonQuery();
                                return rowsAffected > 0;
                            }
                        }
                        else
                        {
                            // Product does not exist, insert new record
                            string insertQuery = "INSERT INTO Products (Name, Price, Description, StockQuantity) " +
                                                 "VALUES (@Name, @Price, @Description, @StockQuantity)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Name", product.Name);
                                insertCommand.Parameters.AddWithValue("@Price", product.Price);
                                insertCommand.Parameters.AddWithValue("@Description", product.Description);
                                insertCommand.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                                int rowsAffected = insertCommand.ExecuteNonQuery();
                                return rowsAffected > 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating/updating product: {ex.Message}");
                return false;
            }
        }


        public bool DeleteCustomer(string customerId)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = "DELETE FROM customers WHERE CustomerId = @CustomerId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }

        }

        public bool DeleteProduct(string productId)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = "DELETE FROM products WHERE product_id = @ProductId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (ProductNotFoundException ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }


        public bool RemoveFromCart(int customerId, int productId)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = "DELETE FROM cart WHERE customer_id = @CustomerId AND product_id = @ProductId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@ProductId", productId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing product from cart: {ex.Message}");
                return false;
            }
        }
        public bool AddToCart(Cart cart)
        {
            try
            {
                using (var connection = DBConnection.GetConnectionString())

                {

                    string checkQuery = "SELECT COUNT(*) FROM Cart WHERE Customer_Id = @CustomerId AND Product_Id = @ProductId";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@CustomerId", cart.CustomerId);
                        checkCommand.Parameters.AddWithValue("@ProductId", cart.ProductId);

                        connection.Open();
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            // Product already exists in the cart, update the quantity
                            string updateQuery = "UPDATE Cart SET Quantity = Quantity + @Quantity " +
                                                 "WHERE Customer_Id = @CustomerId AND Product_Id = @ProductId";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@CustomerId", cart.CustomerId);
                                updateCommand.Parameters.AddWithValue("@ProductId", cart.ProductId);
                                updateCommand.Parameters.AddWithValue("@Quantity", cart.Quantity);

                                int rowsAffected = updateCommand.ExecuteNonQuery();
                                return rowsAffected > 0;
                            }
                        }
                        else
                        {
                            // Product not in the cart, insert a new row
                            string insertQuery = "INSERT INTO Cart (cart_id,Customer_Id, Product_Id, Quantity) " +
                                                 "VALUES (@cartid,@CustomerId, @ProductId, @Quantity)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@cartid", cart.CustomerId);
                                insertCommand.Parameters.AddWithValue("@CustomerId", cart.CustomerId);
                                insertCommand.Parameters.AddWithValue("@ProductId", cart.ProductId);
                                insertCommand.Parameters.AddWithValue("@Quantity", cart.Quantity);

                                int rowsAffected = insertCommand.ExecuteNonQuery();
                                return rowsAffected > 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product to cart: {ex.Message}");
                return false;
            }
        }



        public List<Product> GetAllFromCart(Customer customer)
        {
            List<Product> productsInCart = new List<Product>();

            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = "SELECT p.Product_Id, p.Name, p.Price, p.Description, c.Quantity FROM products p INNER JOIN cart c ON p.Product_Id = c.Product_Id WHERE c.Customer_Id = @CustomerId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["Product_Id"]),
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Description = reader["Description"].ToString(),

                                StockQuantity = Convert.ToInt32(reader["Quantity"])
                            };


                            productsInCart.Add(product);
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products from cart: {ex.Message}");
            }

            return productsInCart;
        }

        public bool PlaceOrder(Order order)
        {
            if (reference.CustomerExists(order.CustomerId))
            {
                Console.WriteLine("You are having items in your cart.");
                List<Product> productsInCart = GetAllFromCart(new Customer { CustomerId = order.CustomerId });
                if (productsInCart.Count > 0)
                {
                    Console.WriteLine("\nProducts in cart:");
                    foreach (var item in productsInCart)
                    {
                        Console.WriteLine($"-ProductId :{item.ProductId}, Product Name : {item.Name}, Description : {item.Description}, QuantityinCart : {item.StockQuantity}");
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Do you want to buy all items from your cart: (yes/no)");
                string buyall = Console.ReadLine();
                if (buyall.ToLower() == "yes")
                {
                    Console.WriteLine("Enter shipping Address:");
                    string ShippingAddress = Console.ReadLine();
                    foreach (var item in productsInCart)
                    {
                        reference.PlaceOrderRef(order.CustomerId, item.ProductId, item.StockQuantity, ShippingAddress);
                    }
                    return true; // Successfully placed order for all items in cart
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Do you want to buy any specific items from your cart: (yes/no)");
                    string buyfromcart = Console.ReadLine();
                    if (buyfromcart.ToLower() == "yes")
                    {
                        Console.WriteLine("Enter the ProductId which you want to buy from cart");
                        int ProductId = int.Parse(Console.ReadLine());
                        int cartquantity = reference.GetQuantityinCart(ProductId, order.CustomerId);
                        Console.WriteLine($"Your cart holds {cartquantity} quantity of ProductID :{ProductId}");
                        Console.WriteLine("Do you want to buy all quantity? (yes/no)");
                        string buyallquantz = Console.ReadLine();
                        if (buyallquantz.ToLower() == "yes")
                        {
                            Console.WriteLine("Enter shipping Address:");
                            string ShippingAddress = Console.ReadLine();
                            reference.PlaceOrderRef(order.CustomerId, ProductId, cartquantity, ShippingAddress);
                            return true; // Successfully placed order for specific item
                        }
                        else
                        {
                            Console.WriteLine("Enter quantity needed:");
                            int newQuantity = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter shipping Address:");
                            string ShippingAddress = Console.ReadLine();
                            reference.PlaceOrderRef(order.CustomerId, ProductId, newQuantity, ShippingAddress);
                            return true; // Successfully placed order for specific quantity of item
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No items in cart. Proceed to Directly buy");
                Console.WriteLine("Enter ProductId:");
                int productid = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Quantity:");
                int newquantity = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Shipping Address:");
                string shippingaddress = Console.ReadLine();
                // Check if the customer exists
                
                if (reference.PlaceOrderRef(order.CustomerId, productid, newquantity, shippingaddress))
                {
                    return true;
                }
                 // Successfully placed order directly
            }
            return false; // Default return if no conditions met
        }

        public List<Order> GetOrdersByCustomer(int customerId)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (var connection = DBConnection.GetConnectionString())
                {
                    string query = @"SELECT o.Order_Id, o.Customer_Id, o.product_id, o.Order_Date, o.Total_Price, o.Shipping_Address, p.name,o.quantity
                             FROM Orders o
                             INNER JOIN Products p ON o.product_id = p.product_id
                             WHERE o.CUSTOMER_ID = @CustomerId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customerId);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                OrderId = reader.IsDBNull(reader.GetOrdinal("Order_Id")) ? 0 : Convert.ToInt32(reader["Order_Id"]),
                                CustomerId = reader.IsDBNull(reader.GetOrdinal("Customer_Id")) ? 0 : Convert.ToInt32(reader["Customer_Id"]),
                                ProductId = reader.IsDBNull(reader.GetOrdinal("product_id")) ? 0 : Convert.ToInt32(reader["product_id"]),
                                OrderDate = reader.IsDBNull(reader.GetOrdinal("Order_Date")) ? DateTime.MinValue : Convert.ToDateTime(reader["Order_Date"]),
                                TotalPrice = reader.IsDBNull(reader.GetOrdinal("Total_Price")) ? 0 : Convert.ToDecimal(reader["Total_Price"]),
                                ShippingAddress = reader.IsDBNull(reader.GetOrdinal("Shipping_Address")) ? string.Empty : reader["Shipping_Address"].ToString(),
                                ProductName = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader["name"].ToString(),
                                quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? 0 : Convert.ToInt32(reader["quantity"])
                            };


                            orders.Add(order);
                        }


                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting orders by customer: {ex.Message}");
            }

            return orders;
        }


    }


    public class reference
    {
        //public static string connstring = "Data Source=LAPTOP-8NH5PHBS;Initial Catalog=Ecommerce;Integrated Security=True";

        public static bool PlaceOrderRef(int customerid, int productid, int quantity, string shippingaddress)
        {
            try
            {
                

                OrderProcessRepositoryImpl orderRepository = new OrderProcessRepositoryImpl();
                if (reference.GetProductStockQuantity(productid) >= quantity)
                {
                    using (var connection = DBConnection.GetConnectionString())
                    {
                        int productprice = reference.GetProductPrice(productid);
                        decimal totalPrice = quantity * productprice;

                        string query = "INSERT INTO orders (customer_id, product_id, quantity, total_price, order_date, shipping_address) " +
                                       "VALUES (@CustomerId, @ProductId, @Quantity, @TotalPrice, @OrderDate, @ShippingAddress)";

                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@CustomerId", customerid);
                        command.Parameters.AddWithValue("@ProductId", productid);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        command.Parameters.AddWithValue("@ShippingAddress", shippingaddress);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            reference.ReduceProductStock(productid, quantity);
                            if (orderRepository.RemoveFromCart(customerid, productid))
                            {
                                Console.WriteLine("Product successfully removed from the cart.");
                            }
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Failed to insert order.");
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Oops! Product is out of stock.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting order: " + ex.Message);
                return false;
            }
        }


        public static bool CustomerExists(int customerId)
        {
            using (var connection = DBConnection.GetConnectionString())
            {
                string query = "select customer_id from cart where customer_id=@customerid";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@customerid", customerId);
                    try
                    {
                        connection.Open(); 
                        SqlDataReader reader = cmd.ExecuteReader();
                        bool exists = reader.HasRows;
                        reader.Close();
                        return exists;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error checking if customer exists: " + ex.Message);
                        return false;
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }
        public static int GetQuantityinCart(int productid, int customerid)
        {
            using (var connection = DBConnection.GetConnectionString())
            {
                string query = "select quantity from cart where customer_id=@customerid and product_id=@productid";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@customerid", customerid);
                    cmd.Parameters.AddWithValue("@productid", productid);
                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("Product not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving cart quantity: " + ex.Message);
                        return 0;
                    }
                }
            }
        }

        public static int GetProductStockQuantity(int productid)
        {
            using (var connection = DBConnection.GetConnectionString())
            {
                string query = "SELECT stockquantity FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productid);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("Product not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving stock quantity: " + ex.Message);
                        return 0;
                    }
                }
            }

        }
        public static bool ReduceProductStock(int productId, int quantityOrdered)
        {
            using (var connection = DBConnection.GetConnectionString())
            {
                string query = "UPDATE products SET stockquantity = stockquantity - @QuantityOrdered WHERE product_id = @ProductId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@QuantityOrdered", quantityOrdered);
                command.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reducing stock quantity: " + ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public static int GetProductPrice(int productId)
        {
            using (var connection = DBConnection.GetConnectionString())
            {
                string query = "SELECT price FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("Product not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error retrieving product price: " + ex.Message);
                        return -1;
                    }
                }
            }
        }

    }
}
