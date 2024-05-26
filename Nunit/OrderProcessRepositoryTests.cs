using E_comm.dao;
using E_comm.entity;
using NUnit.Framework;

namespace Nunit
{
    [TestFixture]
    public class OrderProcessRepositoryTests
    {
        private static OrderProcessRepositoryImpl repository;

        [SetUp]
        public static void Setup()
        {
            repository = new OrderProcessRepositoryImpl();
        }

        [Test]
        public static void Test_Product_Created_Successfully()
        {
            // Arrange
            Product product = new Product
            {
                Name = "Sample1",
                Price = 1080,
                Description = "Product used for testing",
                StockQuantity = 100
            };

            // Act
            bool created = repository.CreateProduct(product);

            // Assert
            Assert.That(created, Is.True, "Product creation should succeed");
        }

        [Test]
        public static void Test_Product_Added_To_Cart_Successfully()
        {
            // Arrange
            Cart cart = new Cart
            {
                CustomerId = 12,
                ProductId = 13,
                Quantity = 1
            };

            // Act
            bool added = repository.AddToCart(cart);

            // Assert
            Assert.That(added, Is.True, "Product should be added to cart successfully");
        }

        public static void Main(string[] args)
        {
            // Run the tests
            Setup();
            Test_Product_Created_Successfully();
            Test_Product_Added_To_Cart_Successfully();
        }
    }
}
