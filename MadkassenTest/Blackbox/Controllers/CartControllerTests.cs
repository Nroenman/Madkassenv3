using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

public class CartControllerTests
{
    // Helper: laver InMemory-DB + CartService + CartController
    // Helper: laver SQLite InMemory-DB + CartService + CartController
    private (CartController controller, ApplicationDbContext context) CreateController(string dbName)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();   // opretter tabellerne

        var cartService = new CartService(context);
        var controller = new CartController(cartService);

        return (controller, context);
    }


    // 1) request == null eller Quantity <= 0  -> 400 "Invalid request."
    [Fact]
    public async Task AddToCart_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var (controller, _) = CreateController(nameof(AddToCart_InvalidRequest_ReturnsBadRequest));

        var request = new AddToCartRequest
        {
            ProductId = 1,
            UserId = 1,
            Quantity = 0 // ugyldig
        };

        // Act
        var result = await controller.AddToCart(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid request.", badRequest.Value);
    }

    // 2) Gyldig request + produkt findes + nok stock  -> 200 "Item added to cart."
    [Fact]
    public async Task AddToCart_ValidRequest_ReturnsOk()
    {
        // Arrange
        var (controller, context) = CreateController(nameof(AddToCart_ValidRequest_ReturnsOk));

        // Seed user med UserId = 1
        context.Users.Add(new Users
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@example.com",
            PasswordHash = "hash",        // tilpas hvis property hedder noget andet
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = "user"
        });

        // Seed produkt med ProductId = 1
        context.Produkter.Add(new Produkter
        {
            ProductId = 1,
            ProductName = "Test product",
            Price = 100,
            StockLevel = 10,
            CategoryId = 1
        });

        await context.SaveChangesAsync();

        var request = new AddToCartRequest
        {
            ProductId = 1,
            UserId = 1,   // matcher UserId ovenfor
            Quantity = 2
        };

        // Act
        var result = await controller.AddToCart(request);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Item added to cart.", ok.Value);
    }

    // 3) Produktet findes ikke  -> CartService kaster "Product not found." -> 400 med samme tekst
    [Fact]
    public async Task AddToCart_ProductNotFound_ReturnsBadRequest()
    {
        // Arrange
        var (controller, _) = CreateController(nameof(AddToCart_ProductNotFound_ReturnsBadRequest));

        var request = new AddToCartRequest
        {
            ProductId = 999, // findes ikke i DB
            UserId = 1,
            Quantity = 1
        };

        // Act
        var result = await controller.AddToCart(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Product not found.", badRequest.Value);
    }

    // 4) Ikke nok stock  -> CartService kaster "Not enough stock available." -> 400 med samme tekst
    [Fact]
    public async Task AddToCart_NotEnoughStock_ReturnsBadRequest()
    {
        // Arrange
        var (controller, context) = CreateController(nameof(AddToCart_NotEnoughStock_ReturnsBadRequest));

        context.Produkter.Add(new Produkter
        {
            ProductId = 1,
            ProductName = "Low stock product",
            Price = 100,
            StockLevel = 2,  // kun 2 på lager
            CategoryId = 1
        });
        await context.SaveChangesAsync();

        var request = new AddToCartRequest
        {
            ProductId = 1,
            UserId = 1,
            Quantity = 5  // beder om mere end lager
        };

        // Act
        var result = await controller.AddToCart(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Not enough stock available.", badRequest.Value);
    }
}
