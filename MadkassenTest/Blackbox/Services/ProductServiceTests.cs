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

        [Fact]
        public async Task AddProductAsync_ProductIsNull_ThrowsArgumentNullException()
        {
            var context = CreateContext();
            var service = new ProductService(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.AddProductAsync(null));
        }

        [Fact]
        public async Task AddProductAsync_ProductNameExists_ThrowsInvalidOperationException()
        {
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

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddProductAsync(newProduct));
        }

        [Fact]
        public async Task AddProductAsync_ValidUniqueProduct_ReturnsProduct()
        {
            var context = CreateContext();
            var service = new ProductService(context);

            var product = new Produkter
            {
                ProductName = "Sprite",
                Price = 10,
                StockLevel = 100,
                CategoryId = 1
            };

            var result = await service.AddProductAsync(product);

            Assert.NotNull(result);
            Assert.Equal("Sprite", result.ProductName);
            Assert.True(context.Produkter.Any(p => p.ProductName == "Sprite"));
        }
    }
}
