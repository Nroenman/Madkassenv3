using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadkassenTest.Whitebox
{
    public class CartTest
    {
        private readonly ApplicationDbContext context;
        public CartTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

            context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddToCartAsync_WhenNoExistingCartItem_AddsNewItemAndReducesStock()
        {
            // Arrange
            int productId = 10;
            int? userId = 1;
            int quantityToAdd = 2;

            var product = new Produkter(
                productId: productId,
                productName: "Cola",
                description: "Soda drink",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 10m,
                stockLevel: 20,
                imageUrl: "cola.png"
            );

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            await service.AddToCartAsync(productId, userId, quantityToAdd);

            // Assert
            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);

            Assert.NotNull(cartItem);
            Assert.Equal(quantityToAdd, cartItem.Quantity);

            var updatedProduct = await context.Produkter.FindAsync(productId);
            Assert.Equal(20 - quantityToAdd, updatedProduct.StockLevel);

            Assert.True(cartItem.AddedAt <= DateTime.UtcNow);
            Assert.True(cartItem.ExpirationTime > cartItem.AddedAt);
        }
        [Fact]
        public async Task AddToCartAsync_WhenExistingCartItem_UpdatesQuantityAndReducesStock()
        {
            // Arrange
            int productId = 20;
            int? userId = 1;
            int initialQuantityInCart = 2;
            int quantityToAdd = 3;

            var product = new Produkter(
                productId: productId,
                productName: "Pepsi",
                description: "Another soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 12m,
                stockLevel: 20,
                imageUrl: "pepsi.png"
            );

            context.Produkter.Add(product);

            var existingCartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = initialQuantityInCart,
                AddedAt = DateTime.UtcNow.AddMinutes(-5),
                ExpirationTime = DateTime.UtcNow.AddMinutes(25)
            };

            context.CartItems.Add(existingCartItem);
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            await service.AddToCartAsync(productId, userId, quantityToAdd);

            // Assert
            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);

            Assert.NotNull(cartItem);
            Assert.Equal(initialQuantityInCart + quantityToAdd, cartItem.Quantity);

            var updatedProduct = await context.Produkter.FindAsync(productId);
            Assert.Equal(20 - quantityToAdd, updatedProduct.StockLevel);
        }
        [Fact]
        public async Task AddToCartAsync_WhenNotEnoughStock_ThrowsInvalidOperationException()
        {
            // Arrange
            int productId = 30;
            int? userId = 1;
            int quantityToAdd = 5;

            var product = new Produkter(
                productId: productId,
                productName: "Fanta",
                description: "Orange soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 9m,
                stockLevel: 2,   // less than quantityToAdd
                imageUrl: "fanta.png"
            );

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AddToCartAsync(productId, userId, quantityToAdd));

            // Ensure nothing was added to cart and stock not changed
            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
            Assert.Null(cartItem);

            var unchangedProduct = await context.Produkter.FindAsync(productId);
            Assert.Equal(2, unchangedProduct.StockLevel);
        }

        [Fact]
        public async Task UpdateCartItemAsync_IncreaseQuantity_UpdatesCartAndReducesStock()
        {
            // Arrange
            int productId = 10;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Cola",
                description: "Soda drink",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 10m,
                stockLevel: 10,
                imageUrl: "cola.png"
            );

            context.Produkter.Add(product);

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2,
                AddedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30)
            };

            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var service = new CartService(context);
            int newQuantity = 5; // 2 -> 5 (extra 3)

            // Act
            await service.UpdateCartItemAsync(productId, userId, newQuantity);

            // Assert
            var updatedCartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
            var updatedProduct = await context.Produkter.FindAsync(productId);

            Assert.NotNull(updatedCartItem);
            Assert.Equal(newQuantity, updatedCartItem.Quantity);

            // stockAdjustment = 2 - 5 = -3 → 10 + (-3) = 7
            Assert.Equal(7, updatedProduct.StockLevel);
        }

        [Fact]
        public async Task UpdateCartItemAsync_DecreaseQuantity_UpdatesCartAndIncreasesStock()
        {
            // Arrange
            int productId = 20;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Pepsi",
                description: "Another soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 12m,
                stockLevel: 5,
                imageUrl: "pepsi.png"
            );

            context.Produkter.Add(product);

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 5,
                AddedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30)
            };

            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var service = new CartService(context);
            int newQuantity = 2; // 5 -> 2 (free 3)

            // Act
            await service.UpdateCartItemAsync(productId, userId, newQuantity);

            // Assert
            var updatedCartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
            var updatedProduct = await context.Produkter.FindAsync(productId);

            Assert.NotNull(updatedCartItem);
            Assert.Equal(newQuantity, updatedCartItem.Quantity);

            // stockAdjustment = 5 - 2 = 3 → 5 + 3 = 8
            Assert.Equal(8, updatedProduct.StockLevel);
        }

        [Fact]
        public async Task UpdateCartItemAsync_WhenNotEnoughStock_ThrowsInvalidOperationException()
        {
            // Arrange
            int productId = 30;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Fanta",
                description: "Orange soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 9m,
                stockLevel: 1, // very low stock
                imageUrl: "fanta.png"
            );

            context.Produkter.Add(product);

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 2, // currently 2 in cart
                AddedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30)
            };

            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var service = new CartService(context);
            int newQuantity = 5; // 2 -> 5 (needs +3)

            // stockAdjustment = 2 - 5 = -3
            // new stock = 1 + (-3) = -2 < 0 → should throw

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateCartItemAsync(productId, userId, newQuantity));

            var unchangedCartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
            var unchangedProduct = await context.Produkter.FindAsync(productId);

            Assert.Equal(2, unchangedCartItem.Quantity);
            Assert.Equal(1, unchangedProduct.StockLevel);
        }

        [Fact]
        public async Task UpdateCartItemAsync_WhenCartItemNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            int productId = 40;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Sprite",
                description: "Lemon-lime soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 8m,
                stockLevel: 10,
                imageUrl: "sprite.png"
            );

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var service = new CartService(context);
            int newQuantity = 3;

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateCartItemAsync(productId, userId, newQuantity));
        }


        [Fact]
        public async Task RemoveCartItemAsync_WhenItemExists_RemovesItemAndRestoresStock()
        {
            // Arrange
            int productId = 10;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Cola",
                description: "Soda drink",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 10m,
                stockLevel: 5,          // current stock
                imageUrl: "cola.png"
            );

            context.Produkter.Add(product);

            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = 3,           // this should be added back to stock
                AddedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30)
            };

            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            await service.RemoveCartItemAsync(productId, userId);

            // Assert
            var removedItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);
            var updatedProduct = await context.Produkter.FindAsync(productId);

            Assert.Null(removedItem);
            // stockLevel should be 5 + 3 = 8
            Assert.Equal(8, updatedProduct.StockLevel);
        }



        [Fact]
        public async Task RemoveCartItemAsync_WhenCartItemNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            int productId = 30;
            int? userId = 1;

            var product = new Produkter(
                productId: productId,
                productName: "Fanta",
                description: "Orange soda",
                categoryId: 1,
                allergies: false,
                allergyType: null,
                price: 9m,
                stockLevel: 7,
                imageUrl: "fanta.png"
            );

            context.Produkter.Add(product);
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RemoveCartItemAsync(productId, userId));

            // Ensure product stock unchanged
            var unchangedProduct = await context.Produkter.FindAsync(productId);
            Assert.Equal(7, unchangedProduct.StockLevel);
        }

        [Fact]
        public async Task GetCartItemsByUserIdAsync_ReturnsCartItemsForGivenUser()
        {
            // Arrange
            int userId = 1;
            int otherUserId = 2;

            var product1 = new Produkter(
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

            var product2 = new Produkter(
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

            context.Produkter.AddRange(product1, product2);

            // Two items for user 1, one for a different user
            context.CartItems.AddRange(
                new CartItem
                {
                    ProductId = product1.ProductId,
                    UserId = userId,
                    Quantity = 2,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30),
                    Produkter = product1
                },
                new CartItem
                {
                    ProductId = product2.ProductId,
                    UserId = userId,
                    Quantity = 3,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30),
                    Produkter = product2
                },
                new CartItem
                {
                    ProductId = product2.ProductId,
                    UserId = otherUserId, // should NOT be returned
                    Quantity = 5,
                    AddedAt = DateTime.UtcNow,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(30),
                    Produkter = product2
                }
            );

            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            var result = await service.GetCartItemsByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // only items for userId = 1

            // Sort by ProductId for predictable assertions
            var ordered = result.OrderBy(r => r.ProductId).ToList();

            var item1 = ordered[0];
            var item2 = ordered[1];

            // First item -> Cola
            Assert.Equal(product1.ProductId, item1.ProductId);
            Assert.Equal(userId, item1.UserId);
            Assert.Equal(2, item1.Quantity);
            Assert.Equal("Cola", item1.ProductName);
            Assert.Equal(10m, item1.Price);

            // Second item -> Pepsi
            Assert.Equal(product2.ProductId, item2.ProductId);
            Assert.Equal(userId, item2.UserId);
            Assert.Equal(3, item2.Quantity);
            Assert.Equal("Pepsi", item2.ProductName);
            Assert.Equal(12m, item2.Price);
        }

    }
}
