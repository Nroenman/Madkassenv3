using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MadkassenTest.Whitebox
{
    public class ReservationExpirationServiceTest
    {
        private static (ServiceProvider provider, SqliteConnection connection) BuildProviderWithSqlite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open(); 

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

            return (services.BuildServiceProvider(), connection);
        }

        private static Task InvokePerformExpirationLogicAsync(
            ReservationExpirationService service,
            CancellationToken token)
        {
            var method = typeof(ReservationExpirationService)
                .GetMethod("PerformExpirationLogic",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
                throw new InvalidOperationException("Could not find PerformExpirationLogic.");

            var result = method.Invoke(service, new object[] { token });

            return (Task)result!;
        }

        [Fact]
        public async Task PerformExpirationLogic_ExpiredItems_RemovedAndStockRestored()
        {
            var (provider, connection) = BuildProviderWithSqlite();

            try
            {
                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await ctx.Database.EnsureCreatedAsync();

                    ctx.Kategori.Add(new Kategori
                    {
                        CategoryId = 1,
                        CategoryName = "Test Category",
                        Description = "Test"
                    });

                    ctx.Users.Add(new Users
                    {
                        UserId = 15,
                        UserName = "Test User",
                        Email = "user@test.com",
                        PasswordHash = "xx"
                    });

                    var product = new Produkter
                    {
                        ProductId = 1,
                        ProductName = "Test product",
                        Price = 100,
                        StockLevel = 5,
                        CategoryId = 1
                    };

                    var cartItem = new CartItem
                    {
                        CartItemId = 1,
                        ProductId = 1,
                        Quantity = 3,
                        UserId = 15,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(-5) // expired
                    };

                    ctx.Produkter.Add(product);
                    ctx.CartItems.Add(cartItem);
                    await ctx.SaveChangesAsync();
                }

                var logger = NullLogger<ReservationExpirationService>.Instance;
                var service = new ReservationExpirationService(provider, logger);

                // Act
                await InvokePerformExpirationLogicAsync(service, CancellationToken.None);

                // Assert
                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var product = await ctx.Produkter.FindAsync(1);
                    var cartItems = await ctx.CartItems.ToListAsync();

                    Assert.NotNull(product);
                    Assert.Equal(8, product!.StockLevel);

                    Assert.Empty(cartItems);
                }
            }
            finally
            {
                connection.Dispose();
            }
        }

        [Fact]
        public async Task PerformExpirationLogic_NoExpiredItems_NoChanges()
        {
            var (provider, connection) = BuildProviderWithSqlite();

            try
            {
                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await ctx.Database.EnsureCreatedAsync();

                    ctx.Kategori.Add(new Kategori
                    {
                        CategoryId = 1,
                        CategoryName = "Test Category",
                        Description = "Test"
                    });

                    ctx.Users.Add(new Users
                    {
                        UserId = 15,
                        UserName = "Test User",
                        Email = "user@test.com",
                        PasswordHash = "xx"
                    });

                    var product = new Produkter
                    {
                        ProductId = 2,
                        ProductName = "Future product",
                        Price = 200,
                        StockLevel = 10,
                        CategoryId = 1
                    };

                    var cartItem = new CartItem
                    {
                        CartItemId = 2,
                        ProductId = 2,
                        Quantity = 4,
                        UserId = 15,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(10) 
                    };

                    ctx.Produkter.Add(product);
                    ctx.CartItems.Add(cartItem);
                    await ctx.SaveChangesAsync();
                }

                var logger = NullLogger<ReservationExpirationService>.Instance;
                var service = new ReservationExpirationService(provider, logger);

                await InvokePerformExpirationLogicAsync(service, CancellationToken.None);

                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var product = await ctx.Produkter.FindAsync(2);
                    var cartItems = await ctx.CartItems.ToListAsync();

                    Assert.NotNull(product);
                    Assert.Equal(10, product!.StockLevel); 
                    Assert.Single(cartItems);              
                    Assert.Equal(2, cartItems[0].CartItemId);
                }
            }
            finally
            {
                connection.Dispose();
            }
        }

        [Fact]
        public async Task PerformExpirationLogic_MixedExpiredAndNonExpired_OnlyExpiredRemoved()
        {
            var (provider, connection) = BuildProviderWithSqlite();

            try
            {
                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await ctx.Database.EnsureCreatedAsync();

                    ctx.Kategori.Add(new Kategori
                    {
                        CategoryId = 1,
                        CategoryName = "Test Category",
                        Description = "Test"
                    });

                    ctx.Users.Add(new Users
                    {
                        UserId = 15,
                        UserName = "Test User",
                        Email = "user@test.com",
                        PasswordHash = "xx"
                    });

                    var product = new Produkter
                    {
                        ProductId = 3,
                        ProductName = "Mixed product",
                        Price = 300,
                        StockLevel = 20,
                        CategoryId = 1
                    };

                    var expiredItem = new CartItem
                    {
                        CartItemId = 3,
                        ProductId = 3,
                        Quantity = 5,
                        UserId = 15,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(-1) 
                    };

                    var notExpiredItem = new CartItem
                    {
                        CartItemId = 4,
                        ProductId = 3,
                        Quantity = 2,
                        UserId = 15, 
                        ExpirationTime = DateTime.UtcNow.AddMinutes(5) 
                    };

                    ctx.Produkter.Add(product);
                    ctx.CartItems.AddRange(expiredItem, notExpiredItem);
                    await ctx.SaveChangesAsync();
                }

                var logger = NullLogger<ReservationExpirationService>.Instance;
                var service = new ReservationExpirationService(provider, logger);

                await InvokePerformExpirationLogicAsync(service, CancellationToken.None);

                using (var scope = provider.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var product = await ctx.Produkter.FindAsync(3);
                    var cartItems = await ctx.CartItems.OrderBy(c => c.CartItemId).ToListAsync();

                    Assert.NotNull(product);
                    Assert.Equal(25, product!.StockLevel);

                    Assert.Single(cartItems);
                    Assert.Equal(4, cartItems[0].CartItemId);
                    Assert.Equal(2, cartItems[0].Quantity);
                }
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}
