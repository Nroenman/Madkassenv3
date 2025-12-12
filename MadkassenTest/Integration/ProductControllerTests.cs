using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace MadkassenTest.Integration
{
    [Collection("Product Tests")]
    public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProductControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private async Task<(int CategoryIdId, int ProductId, string Token)> SeedDatabase()
        {
            var context = GetDbContext();
            context.Produkter.RemoveRange(context.Produkter);
            context.Kategori.RemoveRange(context.Kategori);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            var category = new Kategori
            {
                CategoryName = "Test Category",
                Description = "Test Description"
            };
            context.Kategori.Add(category);
            await context.SaveChangesAsync();

            var product = new Produkter
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 9.99m,
                StockLevel = 100,
                ImageUrl = "test.jpg",
                CategoryId = category.CategoryId
            };
            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var user = new Users
            {
                UserName = "TestUser",
                Email = "testuser@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword"),
                Roles = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginRequest = new AuthInput
            {
                Email = "testuser@test.com",
                Password = "hashedpassword"
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth", loginRequest);
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = loginContent!["token"];
            return (category.CategoryId, product.ProductId, token);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOKWithProducts()
        {
            var (CategoryIdId, ProductId, Token) = await SeedDatabase();

            var response = await _client.GetAsync("/api/product");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<Produkter>>();
            Assert.NotNull(products);
            Assert.Single(products);
            Assert.Equal("Test Product", products[0].ProductName);
        }

        [Fact]
        public async Task AddProduct_ValidRequest_ReturnsCreated()
        {
            var (CategoryIdId, ProductId, Token) = await SeedDatabase();

            var newProduct = new Produkter
            {
                ProductName = "New Product",
                Description = "New Description",
                Price = 19.99m,
                StockLevel = 50,
                ImageUrl = "test.jpg",
                CategoryId = CategoryIdId
            };

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
            var response = await _client.PostAsJsonAsync("/api/product", newProduct);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await response.Content.ReadFromJsonAsync<Produkter>();
            Assert.NotNull(createdProduct);
            Assert.Equal("New Product", createdProduct!.ProductName);
            Assert.True(createdProduct.ProductId > 0);

            var context = GetDbContext();
            var dbProduct = await context.Produkter.FindAsync(createdProduct.ProductId);
            Assert.NotNull(dbProduct);
            Assert.Equal("New Product", dbProduct!.ProductName);
        }

        [Fact]
        public async Task AddProduct_InvalidToken_ReturnsUnauthorized()
        {
            var (CategoryIdId, ProductId, Token) = await SeedDatabase();

            var newProduct = new Produkter
            {
                ProductName = "New Product",
                Description = "New Description",
                Price = 19.99m,
                StockLevel = 50,
                ImageUrl = "test.jpg",
                CategoryId = CategoryIdId
            };

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer InvalidToken");
            var response = await _client.PostAsJsonAsync("/api/product", newProduct);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProduct_ExistingProduct_ReturnsOk()
        {
            var (categoryId, productId, token) = await SeedDatabase();

            var response = await _client.GetAsync($"/api/product/{productId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var product = await response.Content.ReadFromJsonAsync<Produkter>();
            Assert.NotNull(product);
            Assert.Equal(productId, product!.ProductId);
            Assert.Equal("Test Product", product.ProductName);
        }

        [Fact]
        public async Task GetProduct_NonExistentId_ReturnsNotFound()
        {
            var nonExistentProductId = 9999;
            var response = await _client.GetAsync($"/api/product/{nonExistentProductId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProductStock_ValidRequest_ReturnsOk()
        {
            var context = GetDbContext();
            context.Produkter.RemoveRange(context.Produkter);
            context.Kategori.RemoveRange(context.Kategori);
            await context.SaveChangesAsync();

            var category = new Kategori
            {
                CategoryName = "Test Category",
                Description = "Test Description"
            };
            context.Kategori.Add(category);
            await context.SaveChangesAsync();

            var product = new Produkter
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 9.99m,
                StockLevel = 100,
                ImageUrl = "test.jpg",
                CategoryId = category.CategoryId
            };
            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var UpdateRequest = new UpdateStockRequest
            {
                Quantity = 10
            };

            var response = await _client.PutAsJsonAsync($"/api/product/{product.ProductId}", UpdateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedProduct = await response.Content.ReadFromJsonAsync<Produkter>();
            Assert.NotNull(updatedProduct);
            Assert.Equal(110, updatedProduct!.StockLevel);
        }
    }
}