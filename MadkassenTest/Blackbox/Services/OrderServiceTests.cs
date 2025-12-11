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

        [Theory]
        [InlineData(0, 1, 1)]// days min. valid
        [InlineData(1, 1, 1)]// days valid
        [InlineData(30, 0, 0)]// products below lower
        [InlineData(30, 1, 1)]// days max valid + products lower valid + quantity min. valid
        [InlineData(30, 2, 1)]// products above lower
        [InlineData(30, 9, 1)]// products below upper
        [InlineData(30, 10, 1)]// products upper valid
        [InlineData(30, 11, 1)]// products above upper
        [InlineData(30, 1, 0)]// quantity min. invalid
        [InlineData(30, 1, 100)]// quantity max. valid
        public async Task GetTopProductsOverallAsync_BoundaryValueTests(
            int days, int numberOfProducts, int productQuantity)
        {
            var context = CreateContext();

            var orders = new List<Order>();
            for (int i = 0; i < numberOfProducts; i++)
            {
                var order = new Order
                {
                    UserId = i + 1,
                    OrderDate = DateTime.UtcNow.AddDays(-days),
                    OrderStatus = "Pending",
                    TotalAmount = 0
                };
                orders.Add(order);
            }
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            orders = await context.Orders.ToListAsync();

            // Create Produkter entities first
            for (int i = 0; i < numberOfProducts; i++)
            {
                context.Produkter.Add(new Produkter
                {
                    ProductId = i + 1,
                    ProductName = $"Product{i}",
                    Price = 10m,
                    ImageUrl = $"Image{i}.jpg",
                    StockLevel = 100
                });
            }
            await context.SaveChangesAsync();

            // Retrieve Produkter back to ensure proper tracking
            var produkter = await context.Produkter.ToListAsync();

            for (int i = 0; i < numberOfProducts; i++)
            {
                context.OrderItems.Add(new OrderItem
                {
                    OrderId = orders[i].OrderId,
                    Order = orders[i],  // Set Order navigation
                    ProductId = i + 1,
                    Produkter = produkter[i],  // Set Produkter navigation
                    Quantity = productQuantity,
                    ProductName = $"Product{i}",
                    Price = 10m,
                });
            }
            await context.SaveChangesAsync();

            var service = new OrderService(context);
            var topProducts = await service.GetTopProductsOverallAsync(days);

            int expectedCount = numberOfProducts > 10 ? 10 : numberOfProducts;
            if (numberOfProducts == 0 || days == 0)
            {
                expectedCount = 0;
            }

            Assert.Equal(expectedCount, topProducts.Count);

            foreach (var product in topProducts)
            {
                Assert.Equal(productQuantity, product.TotalQuantity);
            }
        }
    }
}