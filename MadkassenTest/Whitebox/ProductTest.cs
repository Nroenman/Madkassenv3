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
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
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
        public void AddProductAsync()
        {

        }



    }
}
