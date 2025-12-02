using System.Net;
using System.Net.Http.Json;
using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTest
{
    public class CartControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CartControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        
        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private async Task<(int UserId, int ProductId)> SeedDatabase()
        {
            var context = GetDbContext();
            context.CartItems.RemoveRange(context.CartItems);
            context.Users.RemoveRange(context.Users);
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

            var user = new Users
            {
                UserName = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Roles = "User",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            };
            context.Users.Add(user);

            var product = new Produkter
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                StockLevel = 100,
                ImageUrl = "test.jpg",
                CategoryId = category.CategoryId
            };
            context.Produkter.Add(product);

            await context.SaveChangesAsync();
            
            return (user.UserId, product.ProductId);
        }

        [Fact]
        public async Task AddToCart_ValidRequest_ReturnsOk()
        {
            var (userId, productId) = await SeedDatabase();

            var request = new AddToCartRequest
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2
            };

            var response = await _client.PostAsJsonAsync("/api/cart/add-to-cart", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("Item added to cart", content);

            var context = GetDbContext();
            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.NotNull(cartItem);
            Assert.Equal(2, cartItem.Quantity);
        }

        [Fact]
        public async Task AddToCart_InvalidQuantity_ReturnsBadRequest()
        {
            var request = new AddToCartRequest
            {
                ProductId = 1,
                UserId = 1,
                Quantity = 0
            };

            var response = await _client.PostAsJsonAsync("/api/cart/add-to-cart", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCartItems_ExistingCart_ReturnsCartItems()
        {
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 3,
                AddedAt = new DateTime(2024, 1, 1),
                ExpirationTime = new DateTime(2024, 1, 2)
            };
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var response = await _client.GetAsync($"/api/cart/get-cart-items?userId={userId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemDto>>();

            Assert.NotNull(cartItems);
            Assert.Single(cartItems);
            Assert.Equal(productId, cartItems[0].ProductId);
            Assert.Equal(3, cartItems[0].Quantity);
        }

        [Fact]
        public async Task GetCartItems_EmptyCart_ReturnsEmptyList()
        {
            var (userId, _) = await SeedDatabase();

            var response = await _client.GetAsync($"/api/cart/get-cart-items?userId={userId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemDto>>();
            
            Assert.NotNull(cartItems);
            Assert.Empty(cartItems);
        }

        [Fact]
        public async Task UpdateCartItem_ValidRequest_ReturnsOk()
        {
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2,
                AddedAt = new DateTime(2024, 1, 1),
                ExpirationTime = new DateTime(2024, 1, 2)
            };
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var updateRequest = new UpdateCartRequest
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 5
            };

            var response = await _client.PutAsJsonAsync("/api/cart/update-cart-item", updateRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var verifyContext = GetDbContext();
            var updatedItem = await verifyContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.NotNull(updatedItem);
            Assert.Equal(5, updatedItem.Quantity);
        }

        [Fact]
        public async Task RemoveCartItem_ValidRequest_ReturnsOk()
        {
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2,
                AddedAt = new DateTime(2024, 1, 1),
                ExpirationTime = new DateTime(2024, 1, 2)
            };
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/cart/remove-cart-item?productId={productId}&userId={userId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Cart item removed successfully", content);

            var verifyContext = GetDbContext();
            var deletedItem = await verifyContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.Null(deletedItem);
        }
    }
}