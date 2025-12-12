using ClassLibrary;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace MadkassenTest.Blackbox.Services
{
    public class UserServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private IConfiguration CreateFakeConfig()
        {
            return new ConfigurationBuilder().AddInMemoryCollection().Build();
        }

        private string CreateJwt(Dictionary<string, string> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims.Select(c => new System.Security.Claims.Claim(c.Key, c.Value))
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // AUTHENTICATE – BLACK BOX TESTS (3 rules)

        // R1: email findes ikke → return null
        [Fact]
        public void Authenticate_EmailDoesNotExist_ReturnsNull()
        {
            // ARRANGE
            var context = CreateContext();
            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            // ACT
            var result = service.Authenticate("unknown@example.com", "Whatever123!");

            // ASSERT
            Assert.Null(result);
        }

        // R2: email findes, password forkert → null
        [Fact]
        public void Authenticate_WrongPassword_ReturnsNull()
        {
            // ARRANGE
            var context = CreateContext();

            context.Users.Add(new Users
            {
                Email = "test@example.com",
                UserName = "Test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct123!")
            });

            context.SaveChanges();

            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            // ACT
            var result = service.Authenticate("test@example.com", "Wrong123!");

            // ASSERT
            Assert.Null(result);
        }

        // R3: email findes, password korrekt → user returneres
        [Fact]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            // ARRANGE
            var context = CreateContext();

            context.Users.Add(new Users
            {
                Email = "test@example.com",
                UserName = "Test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct123!")
            });

            context.SaveChanges();

            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            // ACT
            var result = service.Authenticate("test@example.com", "Correct123!");

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.UserName);
        }


        // GET USER FROM JWT TOKEN – BLACK BOX TESTS (4 rules)

        // R1: Token er IKKE et JWT → SecurityTokenMalformedException
        [Fact]
        public void GetUserFromJwtToken_InvalidToken_Throws() 
        {
            // ARRANGE
            var context = CreateContext();
            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            // ACT & ASSERT
            Assert.Throws<Microsoft.IdentityModel.Tokens.SecurityTokenMalformedException>(() =>
                service.GetUserFromJwtToken("not-a-jwt"));
        }

        // R2: Gyldigt JWT MEN mangler "sub" → UnauthorizedAccessException
        [Fact]
        public void GetUserFromJwtToken_NoSubClaim_Throws()
        {
            // ARRANGE
            var context = CreateContext();
            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            var token = CreateJwt(new Dictionary<string, string>()); // uden sub claim

            // ACT & ASSERT
            Assert.Throws<UnauthorizedAccessException>(() =>
                service.GetUserFromJwtToken(token));
        }

        // R3: Token har "sub", men user findes IKKE → UnauthorizedAccessException
        [Fact]
        public void GetUserFromJwtToken_UserNotFound_Throws()
        {
            // ARRANGE
            var context = CreateContext();
            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            var token = CreateJwt(new Dictionary<string, string>
            {
                { "sub", "GhostUser" }
            });

            // ACT & ASSERT
            Assert.Throws<UnauthorizedAccessException>(() =>
                service.GetUserFromJwtToken(token));
        }

        // R4: Token har "sub" og user findes → return user
        [Fact]
        public void GetUserFromJwtToken_Valid_ReturnsUser()
        {
            // ARRANGE
            var context = CreateContext();

            context.Users.Add(new Users
            {
                UserName = "TestUser",
                Email = "test@example.com",
                PasswordHash = "x"
            });

            context.SaveChanges();

            var config = CreateFakeConfig();
            var service = new UserService(context, config);

            var token = CreateJwt(new Dictionary<string, string>
            {
                { "sub", "TestUser" }
            });

            // ACT
            var result = service.GetUserFromJwtToken(token);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.UserName);
        }
    }
}
