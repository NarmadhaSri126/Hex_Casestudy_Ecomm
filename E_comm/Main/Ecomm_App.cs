using System;
using E_comm.entity;
using E_comm.DBUtil;
using E_comm.exception;
using E_comm.dao;
using E_comm.Main;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_comm.Main
{
    internal class Ecomm_App
    {
        private static OrderProcessRepositoryImpl repository = new OrderProcessRepositoryImpl();
        static void Main(string[] args)
        {
           

            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Register Customer");
                Console.WriteLine("2. Create Product");
                Console.WriteLine("3. Delete Product");
                Console.WriteLine("4. Add to Cart");
                Console.WriteLine("5. View Cart");
                Console.WriteLine("6. Place Order");
                Console.WriteLine("7. View Customer Order");
                Console.WriteLine("8. Exit");

                Console.Write("Enter your choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        RegisterCustomer(repository);
                        break;
                    case 2:
                        CreateProduct(repository);
                        break;
                    case 3:
                        DeleteProduct(repository);
                        break;
                    case 4:
                        AddToCart(repository);
                        break;
                    case 5:
                        ViewCart(repository);
                        break;
                    case 6:
                        PlaceOrder(repository);
                        break;
                    case 7:
                        ViewCustomerOrder(repository);
                        break;
                    case 8:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 8.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void RegisterCustomer(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("Register Customer:");
            Customer customer = new Customer();
            Console.Write("Name: ");
            customer.Name = Console.ReadLine();
            Console.Write("Email: ");
            customer.Email = Console.ReadLine();
            Console.Write("Password: ");
            customer.Password = Console.ReadLine();

            bool customerCreated = repository.CreateCustomer(customer);
            if (customerCreated)
                Console.WriteLine("Customer created successfully");
            else
                Console.WriteLine("Failed to create customer");
        }

        static void CreateProduct(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("Create Product:");
            Product product = new Product();
            Console.Write("Name: ");
            product.Name = Console.ReadLine();
            Console.Write("Price: ");
            product.Price = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Description: ");
            product.Description = Console.ReadLine();
            Console.Write("Stock Quantity: ");
            product.StockQuantity = Convert.ToInt32(Console.ReadLine());

            bool productCreated = repository.CreateProduct(product);
            if (productCreated)
                Console.WriteLine("Product created successfully");
            else
                Console.WriteLine("Failed to create/update product");
        }

        static void DeleteProduct(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("Delete Product:");
            Console.Write("Enter product ID to delete: ");
            string productId = Console.ReadLine();

            bool productDeleted = repository.DeleteProduct(productId);
            if (productDeleted)
                Console.WriteLine("Product deleted successfully");
            else
                Console.WriteLine("Failed to delete product");
        }


        static void AddToCart(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("Add to Cart:");
            Cart cart = new Cart();
            Console.Write("Customer ID: ");
            cart.CustomerId = Convert.ToInt32(Console.ReadLine());
            while (true)
            {

                Console.Write("Product ID: ");
                cart.ProductId = Convert.ToInt32(Console.ReadLine());
                Console.Write("Quantity: ");
                cart.Quantity = Convert.ToInt32(Console.ReadLine());

                bool addedToCart = repository.AddToCart(cart);
                if (addedToCart)
                    Console.WriteLine("Product added to cart successfully");
                else
                    Console.WriteLine("Failed to add product to cart");

                Console.Write("Add another product to the cart? (yes/no): ");
                string input = Console.ReadLine().ToLower();
                if (input != "yes")
                    break;

            }
        }
        static void ViewCart(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("View Cart:");
            Console.Write("Enter customer ID: ");
            int customerId = int.Parse(Console.ReadLine());

            List<Product> productsInCart = repository.GetAllFromCart(new Customer { CustomerId = customerId });
            if (productsInCart.Count > 0)
            {
                Console.WriteLine("\nProducts in cart:");
                foreach (var item in productsInCart)
                {
                    Console.WriteLine($"- Product Name : {item.Name}, Description : {item.Description}, QuantityinCart : {item.StockQuantity}");
                }
            }
            else
            {
                Console.WriteLine("No products in cart.");
            }
        }
        static void PlaceOrder(OrderProcessRepositoryImpl repository)
        {
            try
            {
                Order order=new Order();
                Console.WriteLine("Enter customer ID:");
              order.CustomerId = int.Parse(Console.ReadLine());

                bool orderPlaced = repository.PlaceOrder(order);

                if (orderPlaced)
                {
                    Console.WriteLine("Order placed successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to place order.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void ViewCustomerOrder(OrderProcessRepositoryImpl repository)
        {
            Console.WriteLine("View Customer Order:");
            Console.Write("Enter customer ID: ");
            int customerId = int.Parse(Console.ReadLine());

            List<Order> orders = repository.GetOrdersByCustomer(customerId);

            if (orders.Count > 0)
            {
                Console.WriteLine("\nOrders by customer:");
                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.OrderId}, Order Date: {order.OrderDate},Product: {order.ProductName}, Quantity: {order.quantity},Total Price: {order.TotalPrice}, Shipping Address: {order.ShippingAddress}");
                }
            }
            else
            {
                Console.WriteLine("No orders found for the customer.");
            }
        }
    }
}
