using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class OrderServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Theory]
        //cart item
        [InlineData(0, 1, 1.00, true)] // invalid cartitem (here comes the exception)
        [InlineData(1, 1, 1.00, false)] // min.valid cartitem
        [InlineData(10, 1, 1.00, false)] // valid cartitem

        //quantity
        [InlineData(1, -70, 5.00, false)] // negative quantity
        [InlineData(1, 0, 5.00, false)] // invalid quantity
        [InlineData(1, 1, 5.00, false)] // min.valid quantity
        [InlineData(1, 70, 5.00, false)] // valid quantity

        //price
        [InlineData(1, 1, -56, false)] // negative price
        [InlineData(1, 1, 0.00, false)] // invalid price
        [InlineData(1, 1, 0.01, false)] // min.valid price
        [InlineData(1, 1, 56, false)] // valid price
        public async Task CreateOrder_BoundaryValueTests(
            int cartItemCount,
            int quantity,
            decimal price,
            bool expectException)
        {
            var context = CreateContext();

            for (int i = 0; i < cartItemCount; i++)
            {
                context.CartItems.Add(new CartItem
                {
                    UserId = 1,
                    ProductId = i + 1,
                    Quantity = quantity,
                    Produkter = new Produkter
                    {
                        ProductId = i + 1,
                        ProductName = $"Product{i}",
                        Price = price,
                        StockLevel = 100
                    }
                });
            }

            await context.SaveChangesAsync();

            var service = new OrderService(context);

            if (expectException)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.CreateOrderAsync(1));
            }
            else
            {
                var orderId = await service.CreateOrderAsync(1);

                Assert.True(orderId > 0);

                var order = await context.Orders.FindAsync(orderId);
                decimal expectedTotal = cartItemCount * (quantity * price);

                Assert.Equal(expectedTotal, order.TotalAmount);
            }
        }

        [Theory]
        // TC1: No cart items → expect exception
        [InlineData(false, true, true, true)] // HasCartItems=false
        // TC2: Normal order
        [InlineData(true, true, true, true)] // expectException=false
        // TC3: Missing product
        [InlineData(true, false, true, true)] // ProductExists=false → expect exception
        // TC4: Negative or zero quantity
        [InlineData(true, true, false, true)] // QuantityPositive=false → order created, total may be negative
        // TC5: Negative or zero price
        [InlineData(true, true, true, false)] // PricePositive=false → order created, total may be negative
        public async Task CreateOrder_DecisionTableTests(
            bool hasCartItems,
            bool productExists,
            bool quantityPositive,
            bool pricePositive)
        {
            var context = CreateContext();

            bool expectException = !hasCartItems || !productExists;

            if (hasCartItems)
            {
                int quantity = quantityPositive ? 1 : -1;       // -1 for negative quantity
                decimal price = pricePositive ? 10m : -1m;     // -1 for negative price

                // Add one cart item
                context.CartItems.Add(new CartItem
                {
                    UserId = 1,
                    ProductId = 1,
                    Quantity = quantity,
                    Produkter = productExists
                        ? new Produkter
                        {
                            ProductId = 1,
                            ProductName = "Test Product",
                            Price = price,
                            StockLevel = 100
                        }
                        : null!
                });
            }

            await context.SaveChangesAsync();

            var service = new OrderService(context);

            if (expectException)
            {
                await Assert.ThrowsAnyAsync<Exception>(() =>
                    service.CreateOrderAsync(1));
            }
            else
            {
                var orderId = await service.CreateOrderAsync(1);
                Assert.True(orderId > 0);

                var order = await context.Orders.FindAsync(orderId);

                int cartCount = hasCartItems ? 1 : 0;
                decimal expectedTotal = cartCount * (quantityPositive ? 1 : -1) *
                                        (pricePositive ? 10m : -1m);

                Assert.Equal(expectedTotal, order?.TotalAmount);
            }
        }
    }
}