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

        // partition 1: produkt findes ikke → null
        [Fact]
        public async Task UpdateStock_ProductNotFound_ReturnsNull() // Prevents updating stock for non-existent products
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
        public async Task UpdateStock_PositiveQuantity_IncreasesStock() // Successfully increases stock for existing products
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
        public async Task UpdateStock_ZeroQuantity_NoChange() // No change in stock when quantity is zero
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
        public async Task UpdateStock_NegativeQuantity_EnoughStock_DecreasesStock() // Successfully decreases stock when enough stock is available
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
        public async Task UpdateStock_NegativeQuantity_NotEnoughStock_ReturnsNull() // Prevents stock from going negative
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

        //partition 6: quantity < 0 AND exactly enough stock
        [Fact]
        public async Task UpdateStock_NegativeQuantity_ExactlyEnoughStock_SetsStockToZero() // Sets stock to zero when quantity equals current stock
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
            var result = await service.UpdateProductStockAsync(1, -10);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(0, result.StockLevel);
        }


        // partition 1: product == null
        [Fact]
        public async Task AddProductAsync_ProductIsNull_ThrowsArgumentNullException() // Prevents adding null products
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
        public async Task AddProductAsync_ProductNameExists_ThrowsInvalidOperationException() // Prevents adding duplicate products
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
        public async Task AddProductAsync_ValidUniqueProduct_ReturnsProduct() // Successfully adds a new product
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



        //-----------------------------------------------------------------
        // Unit tests for GetAllProductsAsync

        [Fact]
        public async Task GetAllProductsAsync_NoProducts_ReturnsEmptyList()
        {
            // ARRANGE
            var context = CreateContext();
            var service = new ProductService(context);

            // ACT
            var result = await service.GetAllProductsAsync();

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_WithProducts_ReturnsAllProducts()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.AddRange(
                new Produkter
                {
                    ProductId = 1,
                    ProductName = "Cola",
                    Price = 10,
                    StockLevel = 10,
                    CategoryId = 1
                },
                new Produkter
                {
                    ProductId = 2,
                    ProductName = "Fanta",
                    Price = 12,
                    StockLevel = 5,
                    CategoryId = 2
                }
            );
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.GetAllProductsAsync();

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductId == 1);
            Assert.Contains(result, p => p.ProductId == 2);
        }

        // GetProductByIdAsync

        [Fact]
        public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Cola",
                Price = 10,
                StockLevel = 10,
                CategoryId = 1
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.GetProductByIdAsync(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.Equal("Cola", result.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductDoesNotExist_ReturnsNull()
        {
            // ARRANGE
            var context = CreateContext();
            var service = new ProductService(context);

            // ACT
            var result = await service.GetProductByIdAsync(999);

            // ASSERT
            Assert.Null(result);
        }

        // unit tests for GetProductsByCategoryAsync

        [Fact]
        public async Task GetProductsByCategoryAsync_NoProductsInCategory_ReturnsEmptyList()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Cola",
                Price = 10,
                StockLevel = 10,
                CategoryId = 2
            });
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.GetProductsByCategoryAsync(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ProductsInCategory_ReturnsOnlyCategorySortedByPrice()
        {
            // ARRANGE
            var context = CreateContext();
            context.Produkter.AddRange(
                new Produkter
                {
                    ProductId = 1,
                    ProductName = "Cola",
                    Price = 20,
                    StockLevel = 10,
                    CategoryId = 1
                },
                new Produkter
                {
                    ProductId = 2,
                    ProductName = "Fanta",
                    Price = 10,
                    StockLevel = 5,
                    CategoryId = 1
                },
                new Produkter
                {
                    ProductId = 3,
                    ProductName = "Pepsi",
                    Price = 5,
                    StockLevel = 7,
                    CategoryId = 2 
                }
            );
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // ACT
            var result = await service.GetProductsByCategoryAsync(1);

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(1, p.CategoryId));

            // tjek sortering efter pris (stigende)
            Assert.Equal(10, result[0].Price);
            Assert.Equal(20, result[1].Price);
        }

    }
}
