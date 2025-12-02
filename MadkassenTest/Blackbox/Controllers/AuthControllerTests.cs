using MadkassenRestAPI.Controllers;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using IConfiguration = Castle.Core.Configuration.IConfiguration;

namespace MadkassenTest.Blackbox.Controllers
{
    public class AuthControllerTests
    {
        private IConfigurationRoot CreateFakeConfig()
        {
            var dict = new Dictionary<string, string>
            {
                ["AppSettings:Token"] = "super-secret-key",
                ["AppSettings:Issuer"] = "test-issuer",
                ["AppSettings:Audience"] = "test-audience"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict!)
                .Build();
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithValue()
        {
            // ARRANGE
            var userServiceMock = new Mock<IUserService>();

            var fakeUser = new User
            {
                UserId = 1,
                UserName = "TestUser",
                Email = "test@example.com",
                Roles = "user"
            };

            userServiceMock
                .Setup(s => s.Authenticate("test@example.com", "Correct123!"))
                .Returns(fakeUser);

            var config = CreateFakeConfig();
            var controller = new AuthController(userServiceMock.Object, config);

            var input = new AuthInput
            {
                Email = "test@example.com",
                Password = "Correct123!"
            };

            // ACT
            var result = controller.Login(input);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);              // der kom et body-objekt tilbage
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // ARRANGE
            var userServiceMock = new Mock<IUserService>();

            userServiceMock
                .Setup(s => s.Authenticate("test@example.com", "Wrong123!"))
                .Returns((User?)null);

            var config = CreateFakeConfig();
            var controller = new AuthController(userServiceMock.Object, config);

            var input = new AuthInput
            {
                Email = "test@example.com",
                Password = "Wrong123!"
            };

            // ACT
            var result = controller.Login(input);

            // ASSERT
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.NotNull(unauthorized.Value);         // body findes, detaljer er ikke vigtige her
        }

        [Fact]
        public void Login_AuthenticateThrowsArgumentException_ReturnsUnauthorized()
        {
            // ARRANGE
            var userServiceMock = new Mock<IUserService>();

            userServiceMock
                .Setup(s => s.Authenticate("bad@example.com", "whatever"))
                .Throws(new ArgumentException("Email is required"));

            var config = CreateFakeConfig();
            var controller = new AuthController(userServiceMock.Object, config);

            var input = new AuthInput
            {
                Email = "bad@example.com",
                Password = "whatever"
            };

            // ACT
            var result = controller.Login(input);

            // ASSERT
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.NotNull(unauthorized.Value);
        }

        [Fact]
        public void Login_AuthenticateThrowsUnexpectedException_Returns500()
        {
            // ARRANGE
            var userServiceMock = new Mock<IUserService>();

            userServiceMock
                .Setup(s => s.Authenticate("test@example.com", "Correct123!"))
                .Throws(new Exception("DB down"));

            var config = CreateFakeConfig();
            var controller = new AuthController(userServiceMock.Object, config);

            var input = new AuthInput
            {
                Email = "test@example.com",
                Password = "Correct123!"
            };

            // ACT
            var result = controller.Login(input);

            // ASSERT
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.NotNull(objResult.Value);
        }
    }
}
