using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadkassenTest.Whitebox
{
    public class ProductTest
    {
        private readonly ApplicationDbContext context;
        public ProductTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetproductbyidWhereIdExistAsync()
        {

            context.Database.EnsureCreated();   // opretter tabellerne

            ProductService service = new ProductService(context);

            context.Produkter.Add(new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 10,
                CategoryId = 1
            });

            await context.SaveChangesAsync();


            var result = await service.GetProductByIdAsync(1);

            Assert.IsType<Produkter>(result);
        }
        [Fact]
        public void GetproductbyidWhereIdNotExist()
        {

            context.Database.EnsureCreated();   // opretter tabellerne

            ProductService service = new ProductService(context);

            var result = service.GetProductByIdAsync(99999);

            Assert.Null(result.Result);
        }

        [Fact]
        public async Task AddProductAsync()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            var product = new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 10,
                CategoryId = 1
            };

            var result = await service.AddProductAsync(product);
            Assert.IsType<Produkter>(result);
        }

        [Fact]
        public async Task AddProductAlredyExistAsync()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            var product = new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 10,
                CategoryId = 1
            };

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(
       () => service.AddProductAsync(product));
        }

        [Fact]
        public async Task AddProductWithNullAsync()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            Produkter product = null;

            await Assert.ThrowsAsync<ArgumentNullException>(
       () => service.AddProductAsync(product));
        }

        [Fact]
        public async void UpdateProductStockAsync()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            var product = new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 1,
                CategoryId = 1
            };

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var result = await service.UpdateProductStockAsync(1,10);
            Assert.True(result.StockLevel == 11);
        }
        [Fact]
        public async void UpdateProductStockwith0()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            var product = new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 0,
                CategoryId = 1
            };

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var result = await service.UpdateProductStockAsync(1, -1);
            Assert.Null(result);
        }
        [Fact]
        public async void UpdateProductStockwithnull()
        {
            context.Database.EnsureCreated();

            ProductService service = new ProductService(context);

            Produkter product = null;

            var result = await service.UpdateProductStockAsync(1, 1);
            Assert.Null(result);
        }
        [Fact]
        public async void GetProductsByCategoryAsync()
        {
            context.Database.EnsureCreated();

            var service = new ProductService(context);

            var product = new Produkter
            {
                ProductId = 1,
                ProductName = "Test product",
                Price = 100,
                StockLevel = 0,
                CategoryId = 1
            };

            var category = new Kategori
            {
                CategoryId = 1,
                CategoryName = "Category",
                Description = "Description",
            };

            context.Kategori.Add(category);

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var result = await service.GetProductsByCategoryAsync(1);
            Assert.IsType<List<Produkter>>(result);
        }
    }
}
