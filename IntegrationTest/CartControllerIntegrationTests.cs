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
    /// <summary>
    /// Integration test class for CartController.
    /// IClassFixture<CustomWebApplicationFactory> tells xUnit to:
    /// 1. Create ONE instance of CustomWebApplicationFactory for ALL tests in this class
    /// 2. Share it across all tests (more efficient than creating for each test)
    /// 3. Properly dispose of it when all tests complete
    /// </summary>
    public class CartControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        /// <summary>
        /// Constructor - xUnit automatically injects the CustomWebApplicationFactory
        /// This runs before EACH test method
        /// </summary>
        public CartControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory; // Store factory so we can access the in-memory database
            _client = factory.CreateClient(); // Create an HttpClient that sends requests to our test server
        }

        /// <summary>
        /// Helper method to get database context from the test server
        /// </summary>
        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        /// <summary>
        /// Helper method to set up test data in the real SQL Server database
        /// Cleans up test data before each test to ensure clean state
        /// Returns the generated UserId and ProductId
        /// </summary>
        private async Task<(int UserId, int ProductId)> SeedDatabase()
        {
            var context = GetDbContext();
            
            // STEP 1: Clean up test data (delete test records if they exist)
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == 1);
            if (existingUser != null) context.Users.Remove(existingUser);
            
            var existingProduct = await context.Produkter.FirstOrDefaultAsync(p => p.ProductId == 1);
            if (existingProduct != null) context.Produkter.Remove(existingProduct);
            
            var existingCartItems = context.CartItems.Where(c => c.UserId == 1);
            context.CartItems.RemoveRange(existingCartItems);
            
            await context.SaveChangesAsync();

            // STEP 2: Add test user (don't set UserId - let database auto-generate it)
            var user = new Users
            {
                UserName = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Roles = "User",
                CreatedAt = new DateTime(2024, 1, 1), // Use a valid datetime (not datetime2) value
                UpdatedAt = new DateTime(2024, 1, 1)
            };
            context.Users.Add(user);

            // STEP 3: Add test product (don't set ProductId - let database auto-generate it)
            var product = new Produkter
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                StockLevel = 100,
                ImageUrl = "test.jpg",
                CategoryId = 1
            };
            context.Produkter.Add(product);

            // STEP 4: Save to SQL Server database - IDs will be auto-generated
            await context.SaveChangesAsync();
            
            // Return the generated IDs
            return (user.UserId, product.ProductId);
        }

        /// <summary>
        /// TEST 1: Add item to cart with valid request
        /// Tests the happy path where everything works correctly
        /// </summary>
        [Fact] // [Fact] marks this as a test method that xUnit should run
        public async Task AddToCart_ValidRequest_ReturnsOk()
        {
            // ARRANGE: Set up test data and prepare the request
            // ================================================
            
            // Seed the database with a user and product
            var (userId, productId) = await SeedDatabase();

            // Create the request object to add 2 items to cart
            var request = new AddToCartRequest
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2
            };

            // ACT: Execute the action we're testing
            // ======================================
            
            // Send POST request to the API endpoint
            // PostAsJsonAsync automatically serializes the object to JSON
            var response = await _client.PostAsJsonAsync("/api/cart/add-to-cart", request);

            // ASSERT: Verify the results
            // ==========================
            
            // Check that HTTP status code is 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // Read the response body as a string
            var content = await response.Content.ReadAsStringAsync();
            
            // Verify the success message is in the response
            Assert.Contains("Item added to cart", content);

            // Verify the database was updated correctly
            var context = GetDbContext();
            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.NotNull(cartItem); // Cart item should exist
            Assert.Equal(2, cartItem.Quantity); // Quantity should be 2
        }

        /// <summary>
        /// TEST 2: Add item to cart with invalid quantity (0 or negative)
        /// Tests validation logic - should reject invalid requests
        /// </summary>
        [Fact]
        public async Task AddToCart_InvalidQuantity_ReturnsBadRequest()
        {
            // ARRANGE: Create a request with invalid quantity (0)
            var request = new AddToCartRequest
            {
                ProductId = 1,
                UserId = 1,
                Quantity = 0 // Invalid - quantity must be positive
            };

            // ACT: Send the request
            var response = await _client.PostAsJsonAsync("/api/cart/add-to-cart", request);

            // ASSERT: Should return 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// TEST 3: Get cart items for a user who has items in cart
        /// Tests that the API returns cart items with correct product details
        /// </summary>
        [Fact]
        public async Task GetCartItems_ExistingCart_ReturnsCartItems()
        {
            // ARRANGE: Set up a cart with one item
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            // Manually add a cart item to the database
            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 3,
                AddedAt = new DateTime(2024, 1, 1),  // Must set datetime for SQL Server datetime columns
                ExpirationTime = new DateTime(2024, 1, 2)
            };
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            // ACT: Request cart items for the user
            var response = await _client.GetAsync($"/api/cart/get-cart-items?userId={userId}");

            // ASSERT: Verify response
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // Deserialize JSON response to List of CartItemDto
            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemDto>>();
            
            Assert.NotNull(cartItems); // Should not be null
            Assert.Single(cartItems); // Should have exactly 1 item
            Assert.Equal(productId, cartItems[0].ProductId); // Product ID should match
            Assert.Equal(3, cartItems[0].Quantity); // Quantity should be 3
        }

        /// <summary>
        /// TEST 4: Get cart items for a user with empty cart
        /// Tests that API returns empty list (not null or error) for empty cart
        /// </summary>
        [Fact]
        public async Task GetCartItems_EmptyCart_ReturnsEmptyList()
        {
            // ARRANGE: Set up database but don't add any cart items
            var (userId, _) = await SeedDatabase();
            // Note: Not adding any cart items - cart is empty

            // ACT: Request cart items for the user
            var response = await _client.GetAsync($"/api/cart/get-cart-items?userId={userId}");

            // ASSERT: Should return OK with empty list
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemDto>>();
            
            Assert.NotNull(cartItems); // List should exist (not null)
            Assert.Empty(cartItems); // But should be empty (no items)
        }

        /// <summary>
        /// TEST 5: Update quantity of an existing cart item
        /// Tests that we can modify the quantity of items already in the cart
        /// </summary>
        [Fact]
        public async Task UpdateCartItem_ValidRequest_ReturnsOk()
        {
            // ARRANGE: Set up cart with existing item
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            // Add cart item with quantity 2
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

            // Create request to update quantity to 5
            var updateRequest = new UpdateCartRequest
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 5
            };

            // ACT: Send PUT request to update the cart item
            var response = await _client.PutAsJsonAsync("/api/cart/update-cart-item", updateRequest);

            // ASSERT: Verify update was successful
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify database was updated
            var verifyContext = GetDbContext();
            var updatedItem = await verifyContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.NotNull(updatedItem); // Item should still exist
            Assert.Equal(5, updatedItem.Quantity); // Quantity should now be 5 (updated from 2)
        }

        /// <summary>
        /// TEST 6: Remove an item from the cart
        /// Tests that we can delete cart items completely
        /// </summary>
        [Fact]
        public async Task RemoveCartItem_ValidRequest_ReturnsOk()
        {
            // ARRANGE: Set up cart with an item to remove
            var (userId, productId) = await SeedDatabase();
            var context = GetDbContext();

            // Add cart item to be removed
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

            // ACT: Send DELETE request
            // Using query parameters with actual IDs
            var response = await _client.DeleteAsync($"/api/cart/remove-cart-item?productId={productId}&userId={userId}");

            // ASSERT: Verify deletion was successful
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Cart item removed successfully", content);

            // Verify item no longer exists in database
            var verifyContext = GetDbContext();
            var deletedItem = await verifyContext.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            
            Assert.Null(deletedItem); // Item should be gone (null)
        }
    }
}