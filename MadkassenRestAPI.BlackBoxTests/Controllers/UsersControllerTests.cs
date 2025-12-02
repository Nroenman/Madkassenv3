using ClassLibrary;
using ClassLibrary.Model;                 // DTO: Users
using MadkassenRestAPI.Controllers;
using MadkassenRestAPI.Models;          // Entity: User + ApplicationDbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MadkassenRestAPI.BlackBoxTests.Controllers
{
    public class UsersControllerTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private IConfiguration CreateFakeConfiguration()
        {
            var dict = new Dictionary<string, string>();
            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsOkWithUser()
        {
            // ARRANGE
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var userEntity = new User
            {
                UserId = 1,
                UserName = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Roles = "user"
            };
            context.Users.Add(userEntity);
            await context.SaveChangesAsync();

            var config = CreateFakeConfiguration();
            var controller = new UsersController(context, config);

            // ACT
            var result = await controller.GetUserById(1);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<Users>(okResult.Value);

            Assert.Equal(1, dto.UserId);
            Assert.Equal("TestUser", dto.UserName);
            Assert.Equal("test@example.com", dto.Email);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            // ARRANGE
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var config = CreateFakeConfiguration();
            var controller = new UsersController(context, config);

            // ACT
            var result = await controller.GetUserById(9999);

            // ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
