using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class CartServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new ApplicationDbContext(options);
        }

        [Theory]
        [InlineData(false, false, 5, 10, true, "Product not found.")]
        [InlineData(true, false, 15, 10, true, "Not enough stock available.")]
        [InlineData(true, false, 5, 10, false, null)]
        [InlineData(true, true, 5, 20, false, null)]
        
        public async Task AddToCartAsync_DecisionTableTests(
            bool productExists,
            bool cartItemExists,
            int quantity,
            int stockLevel,
            bool expectException,
            string expectedExceptionMessage)
        {
            var context = CreateContext();
            var service = new CartService(context);
            int productId = 1;
            int userId = 1;

            if (productExists)
            {
                context.Produkter.Add(new Produkter
                {
                    ProductId = productId,
                    ProductName = "Test Product",
                    Price = 10m,
                    StockLevel = stockLevel,
                    ImageUrl = "test.jpg"
                });
                await context.SaveChangesAsync();
            }

            if (cartItemExists)
            {
                context.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = 2,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30)
                });
                await context.SaveChangesAsync();
            }

            if (expectException)
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.AddToCartAsync(productId, userId, quantity));

                Assert.Equal(expectedExceptionMessage, exception.Message);
            }
            else
            {
                await service.AddToCartAsync(productId, userId, quantity);

                var cartItem = await context.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);

                Assert.NotNull(cartItem);

                if (cartItemExists)
                {
                    Assert.Equal(2 + quantity, cartItem.Quantity);
                }
                else
                {
                    Assert.Equal(quantity, cartItem.Quantity);
                }

                var product = await context.Produkter.FindAsync(productId);
                Assert.Equal(stockLevel - quantity, product.StockLevel);
            }
        }

        [Theory]
        [InlineData(false, true, 0, 10, 10, true, "Cart item not found.")]
        [InlineData(true, false, 5, 10, 10, true, "Product not found.")]
        [InlineData(true, true, 5, 20, 10, true, "Not enough stock available.")]
        [InlineData(true, true, 5, 10, 10, false, null)]
        
        public async Task UpdateCartItemAsync_DecisionTableTests(
            bool cartItemExists,
            bool productExists,
            int currentQuantity,
            int newQuantity,
            int stockLevel,
            bool expectException,
            string expectedExceptionMessage)
        {
            var context = CreateContext();
            var service = new CartService(context);
            int productId = 1;
            int userId = 1;

            if (productExists)
            {
                context.Produkter.Add(new Produkter
                {
                    ProductId = productId,
                    ProductName = "Test Product",
                    Price = 10m,
                    StockLevel = stockLevel,
                    ImageUrl = "test.jpg"
                });
                await context.SaveChangesAsync();
            }

            if (cartItemExists)
            {
                context.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = currentQuantity,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30)
                });
                await context.SaveChangesAsync();
            }

            if (expectException)
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.UpdateCartItemAsync(productId, userId, newQuantity));

                Assert.Equal(expectedExceptionMessage, exception.Message);
            }
            else
            {
                await service.UpdateCartItemAsync(productId, userId, newQuantity);

                var cartItem = await context.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);

                Assert.NotNull(cartItem);
                Assert.Equal(newQuantity, cartItem.Quantity);

                var product = await context.Produkter.FindAsync(productId);
                int stockAdjustment = currentQuantity - newQuantity;
                Assert.Equal(stockLevel + stockAdjustment, product.StockLevel);
            }
        }
        
        [Theory]
        [InlineData(false, true,  1,    1,    2, 10, true,  "Cart item not found.")]
        [InlineData(true,  true,  1,    1,    2, 10, false, null)]
        [InlineData(true,  false, 1,    1,    2, 10, false, null)]
        [InlineData(true,  true,  2,    1,    2, 10, true,  "Cart item not found.")]
        
        public async Task RemoveCartItemAsync_EquivalencePartitionTests(
            bool cartItemExists,
            bool productExists,
            int? cartItemUserId,
            int? callUserId,
            int cartQuantity,
            int stockLevel,
            bool expectException,
            string expectedExceptionMessage)
        {
            var context = CreateContext();
            var service = new CartService(context);
            int productId = 1;

            if (productExists)
            {
                context.Produkter.Add(new Produkter
                {
                    ProductId = productId,
                    ProductName = "Test Product",
                    Price = 10m,
                    StockLevel = stockLevel,
                    ImageUrl = "test.jpg"
                });

                await context.SaveChangesAsync();
            }

            if (cartItemExists)
            {
                context.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    UserId = cartItemUserId,
                    Quantity = cartQuantity,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30)
                });

                await context.SaveChangesAsync();
            }

            if (expectException)
            {
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.RemoveCartItemAsync(productId, callUserId));

                Assert.Equal(expectedExceptionMessage, ex.Message);
                return;
            }

            await service.RemoveCartItemAsync(productId, callUserId);
            
            bool stillThere = await context.CartItems.AnyAsync(ci =>
                ci.ProductId == productId && ci.UserId == callUserId);

            Assert.False(stillThere);
            
            var product = await context.Produkter.FindAsync(productId);

            if (productExists)
            {
                Assert.NotNull(product);
                Assert.Equal(stockLevel + cartQuantity, product!.StockLevel);
            }
            else
            {
                Assert.Null(product);
            }
        }
    }
}