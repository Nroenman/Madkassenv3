using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class ProductServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // ----------------------------------------------------------
        // UPDATE STOCK TESTS (5 partitions)
        // ----------------------------------------------------------

        // partition 1: produkt findes ikke → null
        [Fact]
        public async Task UpdateStock_ProductNotFound_ReturnsNull()
        {
            // ARRANGE
            var context = CreateContext();
            var service = new ProductService(context);

            // ACT
            var result = await service.UpdateProductStockAsync(999, 5);

            // ASSERT
            Assert.Null(result);
        }

        // partition 2: quantity > 0
        [Fact]
        public async Task UpdateStock_PositiveQuantity_IncreasesStock()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.UpdateProductStockAsync(1, 5);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(15, result.StockLevel);
        }

        // partition 3: quantity == 0
        [Fact]
        public async Task UpdateStock_ZeroQuantity_NoChange()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.UpdateProductStockAsync(1, 0);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(10, result.StockLevel);
        }

        // partition 4: quantity < 0 AND enough stock
        [Fact]
        public async Task UpdateStock_NegativeQuantity_EnoughStock_DecreasesStock()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.UpdateProductStockAsync(1, -5);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(5, result.StockLevel);
        }

        // partition 5: quantity < 0 AND NOT enough stock → null
        [Fact]
        public async Task UpdateStock_NegativeQuantity_NotEnoughStock_ReturnsNull()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 3
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.UpdateProductStockAsync(1, -5);

            // ASSERT
            Assert.Null(result);
        }


        // ----------------------------------------------------------
        // ADD PRODUCT TESTS (3 partitions)
        // ----------------------------------------------------------

        // partition 1: product == null
        [Fact]
        public async Task AddProductAsync_ProductIsNull_ThrowsArgumentNullException()
        {
            // ARRANGE
            var context = CreateContext();
            var service = new ProductService(context);

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.AddProductAsync(null));
        }

        // partition 2: produktnavn findes allerede
        [Fact]
        public async Task AddProductAsync_ProductNameExists_ThrowsInvalidOperationException()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter { ProductName = "Cola" });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            var newProduct = new Produkter
            {
                ProductName = "Cola",
                Price = 10,
                StockLevel = 5,
                CategoryId = 1
            };

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddProductAsync(newProduct));
        }

        // partition 3: gyldigt produkt
        [Fact]
        public async Task AddProductAsync_ValidUniqueProduct_ReturnsProduct()
        {
            // ARRANGE
            var context = CreateContext();
            var service = new ProductService(context);

            var product = new Produkter
            {
                ProductName = "Sprite",
                Price = 10,
                StockLevel = 100,
                CategoryId = 1
            };

            // ACT
            var result = await service.AddProductAsync(product);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Sprite", result.ProductName);
            Assert.True(context.Produkter.Any(p => p.ProductName == "Sprite"));
        }
    }
}
