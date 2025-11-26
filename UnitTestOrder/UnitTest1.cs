using Microsoft.EntityFrameworkCore;
using MadkassenRestAPI.Services;
using ClassLibrary.Model;
using Xunit;
using ClassLibrary;
using MadkassenRestAPI.Models;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace UnitTest
{
    public class OrderServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ApplicationDbContext _dbContext;
        private readonly OrderService _orderService;

        public OrderServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _orderService = new OrderService(_dbContext);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsCorrectOrderId()
        {
            // Arrange: Add a user and cart item to the in-memory database
            var userId = 1;
    
            var product = new Produkter
            {
                ProductName = "Test Product",
                Price = 100,
            };

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = 1,
                Quantity = 2,
                Produkter = product
            };

            _dbContext.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();

            // Act: Create order
            var orderId = await _orderService.CreateOrderAsync(userId);

            // Assert: Ensure that the order was created
            Assert.True(orderId > 0);
            var order = await _dbContext.Orders.FindAsync(orderId);
            Assert.NotNull(order);

            _testOutputHelper.WriteLine($"Order ID: {orderId}");
        }
    }
}