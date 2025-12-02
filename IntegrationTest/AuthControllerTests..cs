using System.Net;
using System.Net.Http.Json;
using BCrypt.Net;
using ClassLibrary;
using ClassLibrary.Model;
using MadkassenRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private ApplicationDbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private async Task<int> seedDatabase()
        {
            var context = GetDbContext();
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            var user = new Users
            {
                UserName = "TestUser",
                Email = "testuser@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpassword"),
                Roles = "User",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user.UserId;
        }

        [Fact]

        public async Task Login_ValidCredentials_ReturnsOkAndToken()
        {
            await seedDatabase();

            var loginRequest = new AuthInput
            {
                Email = "testuser@test.com",
                Password = "testpassword"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("token"));
            Assert.False(string.IsNullOrEmpty(result["token"]));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            await seedDatabase();

            var loginRequest = new AuthInput
            {
                Email = "testuser@test.com",
                Password = "wrongpassword"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("message"));
            Assert.False(string.IsNullOrEmpty(result["message"]));
        }
    }
}