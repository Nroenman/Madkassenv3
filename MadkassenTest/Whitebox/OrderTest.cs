using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadkassenTest.Whitebox
{
    public class OrderTest
    {
        private readonly ApplicationDbContext context;
        public OrderTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;

            context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateOrderAsync()
        {
            await context.SaveChangesAsync();

            OrderService service = new OrderService(context);

            var user = new User
            {
                UserId = 1,
                Email = "test@test.dk",
                PasswordHash = Guid.NewGuid().ToString(),
                UserName = "TestUser"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateOrderAsync(user.UserId));
        }

        [Fact]
        public async Task GetTopProductsByUserAsync()
        {// Arrange

            OrderService service = new OrderService(context);

            int userId = 1;

            var user = new User
            {
                UserId = userId,
                Email = "test@test.dk",
                PasswordHash = Guid.NewGuid().ToString(),
                UserName = "TestUser"
            };

            context.Users.Add(user);

            // --- Seed orders within last 7 days ---
            var order1 = new Order
            {
                UserId = userId,
                Users = user,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                OrderStatus = "Completed",
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                UserId = userId,
                Users = user,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                OrderStatus = "Completed",
                TotalAmount = 150m
            };

            context.Orders.AddRange(order1, order2);

            // --- Seed products ---
            var prod1 = new Produkter(
            productId: 10,
            productName: "Cola",
            description: "Soda drink",
            categoryId: 1,
            allergies: false,
            allergyType: null,
            price: 10.0m,
            stockLevel: 100,
            imageUrl: "cola.png"
    );

            var prod2 = new Produkter(
                productId: 11,
                productName: "Pepsi",
                description: "Another soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 10.0m,
                stockLevel: 100,
                imageUrl: "pepsi.png"
            );

            context.Produkter.AddRange(prod1, prod2);

            // --- Order items ---
            context.OrderItems.AddRange(
        new OrderItem
        {
            OrderId = 100,
            Order = order1,               // required for .Order.OrderDate
            ProductId = prod2.ProductId,
            ProductName = prod2.ProductName,
            Quantity = 3,
            Price = prod2.Price,
            Produkter = prod2
        },
        new OrderItem
        {
            OrderId = 101,
            Order = order2,
            ProductId = prod1.ProductId,
            ProductName = prod1.ProductName,
            Quantity = 2,
            Price = prod1.Price,
            Produkter = prod1
        },
        new OrderItem
        {
            OrderId = 101,
            Order = order2,
            ProductId = prod1.ProductId,
            ProductName = prod1.ProductName,
            Quantity = 4,
            Price = prod1.Price,
            Produkter = prod1
        }
    );

            await context.SaveChangesAsync();



            // Act
            var result = await service.GetTopProductsByUserAsync(userId, days: 7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            // Product with highest quantity should be first prod1. (2 orders 2 + 4 = 6) 
            Assert.Equal(10, result[0].ProductId);
            Assert.Equal(6, result[0].TotalQuantity);

            // prod2 should be second 
            Assert.Equal(11, result[1].ProductId);
            Assert.Equal(3, result[1].TotalQuantity);
        }

        [Fact]
        public async Task GetTopProductsOverallAsync_ReturnsCorrectTopProducts()
        {
            // Arrange
            int userId = 1;

            var user = new Users
            {
                UserId = userId,
                Email = "test@test.dk",
                PasswordHash = Guid.NewGuid().ToString(),
                UserName = "TestUser"
            };
            context.Users.Add(user);

            // Orders in date range
            var order1 = new Order
            {
                UserId = userId,
                Users = user,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                OrderStatus = "Completed",
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                UserId = userId,
                Users = user,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                OrderStatus = "Completed",
                TotalAmount = 150m
            };

            context.Orders.AddRange(order1, order2);

            // Products
            var cola = new Produkter(
                productId: 10,
                productName: "Cola",
                description: "Soda drink",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 10m,
                stockLevel: 100,
                imageUrl: "cola.png"
            );

            var pepsi = new Produkter(
                productId: 11,
                productName: "Pepsi",
                description: "Another soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 12m,
                stockLevel: 80,
                imageUrl: "pepsi.png"
            );

            context.Produkter.AddRange(cola, pepsi);

            // OrderItems (same structure as before)
            // Cola = 3 + 2 = 5  
            // Pepsi = 4
            context.OrderItems.AddRange(
                new OrderItem
                {
                    Order = order1,
                    OrderId = order1.OrderId,
                    ProductId = cola.ProductId,
                    ProductName = cola.ProductName,
                    Quantity = 3,
                    Price = cola.Price,
                    Produkter = cola
                },
                new OrderItem
                {
                    Order = order2,
                    OrderId = order2.OrderId,
                    ProductId = cola.ProductId,
                    ProductName = cola.ProductName,
                    Quantity = 2,
                    Price = cola.Price,
                    Produkter = cola
                },
                new OrderItem
                {
                    Order = order2,
                    OrderId = order2.OrderId,
                    ProductId = pepsi.ProductId,
                    ProductName = pepsi.ProductName,
                    Quantity = 4,
                    Price = pepsi.Price,
                    Produkter = pepsi
                }
            );

            await context.SaveChangesAsync();

            var service = new OrderService(context);

            // Act
            var result = await service.GetTopProductsOverallAsync(days: 7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            // Results must be sorted by total quantity DESC
            var first = result[0];
            var second = result[1];

            // Cola is first: total = 5
            Assert.Equal(cola.ProductId, first.ProductId);
            Assert.Equal("Cola", first.ProductName);
            Assert.Equal("cola.png", first.ImageUrl);
            Assert.Equal(5, first.TotalQuantity);

            // Pepsi is second: total = 4
            Assert.Equal(pepsi.ProductId, second.ProductId);
            Assert.Equal("Pepsi", second.ProductName);
            Assert.Equal("pepsi.png", second.ImageUrl);
            Assert.Equal(4, second.TotalQuantity);
        }

    }
}
