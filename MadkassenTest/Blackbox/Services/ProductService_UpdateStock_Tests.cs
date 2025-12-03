using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class ProductService_UpdateStock_Tests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // partition 1: produkt findes ikke → null
        [Fact]
        public async Task UpdateStock_ProductNotFound_ReturnsNull()
        {
            var context = CreateContext();
            var service = new ProductService(context);

            var result = await service.UpdateProductStockAsync(999, 5);

            Assert.Null(result);
        }

        // partition 2: quantity > 0
        [Fact]
        public async Task UpdateStock_PositiveQuantity_IncreasesStock()
        {
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            var result = await service.UpdateProductStockAsync(1, 5);

            Assert.NotNull(result);
            Assert.Equal(15, result.StockLevel);
        }

        // partition 3: quantity == 0
        [Fact]
        public async Task UpdateStock_ZeroQuantity_NoChange()
        {
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            var result = await service.UpdateProductStockAsync(1, 0);

            Assert.NotNull(result);
            Assert.Equal(10, result.StockLevel);
        }

        // partition 4: quantity < 0 and enough stock
        [Fact]
        public async Task UpdateStock_NegativeQuantity_EnoughStock_DecreasesStock()
        {
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 10
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            var result = await service.UpdateProductStockAsync(1, -5);

            Assert.NotNull(result);
            Assert.Equal(5, result.StockLevel);
        }

        // partition 5: quantity < 0 and NOT enough stock
        [Fact]
        public async Task UpdateStock_NegativeQuantity_NotEnoughStock_ReturnsNull()
        {
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test",
                StockLevel = 3
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            var result = await service.UpdateProductStockAsync(1, -5);

            Assert.Null(result);
        }
    }
}
