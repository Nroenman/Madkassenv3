using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClassLibrary;
using MadkassenRestAPI.Models;
using MadkassenRestAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MadkassenTest.Whitebox;

public class UserTest

{
    private readonly ApplicationDbContext context;

    private ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
    
    
    [Fact]
    public void Authenticate_UserNotFound_ReturnsNull()
    {
        var db = CreateDb();
        var service = new UserService(db, null);

        var result = service.Authenticate("none@test.com", "123");

        Assert.Null(result);
    }
    
    
    [Fact]
    public void Authenticate_WrongPassword_ReturnsNull()
    {
        var db = CreateDb();

        db.Users.Add(new Users
        {
            UserId = 1075,
            UserName = "Test User",
            Email = "Test1@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct")
        });
        db.SaveChanges();

        var service = new UserService(db, null);

        var result = service.Authenticate("test@test.com", "wrong");

        Assert.Null(result);
    }
    
    
    [Fact]
    public void Authenticate_ValidCredentials_ReturnsUser()
    {
        var db = CreateDb();

        db.Users.Add(new Users
        {
            UserId = 1075,
            UserName = "Test User",
            Email = "Test1@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test1@test.com")
        });
        db.SaveChanges();

        var service = new UserService(db, null);

        var result = service.Authenticate("Test1@test.com", "Test1@test.com");

        Assert.NotNull(result);
        Assert.Equal("Test User", result.UserName);
    }

        //hjælper for at lave falske JWTS
    private string CreateJwt(string sub)
    {
        var token = new JwtSecurityToken(
            claims: new[] { new Claim("sub", sub) }
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [Fact]
    public void GetUserFromJwtToken_InvalidToken_Throws()
    {
        var db = CreateDb();
        var service = new UserService(db, null);

        Assert.Throws<ArgumentException>(() =>
            service.GetUserFromJwtToken("bad.token.value"));
    }
    [Fact]
    public void GetUserFromJwtToken_NoSubClaim_Throws()
    {
        var db = CreateDb();
        var token = new JwtSecurityToken().ToString(); 
        var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken());

        var service = new UserService(db, null);

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.GetUserFromJwtToken(jwt));
    }

    [Fact]
    public void GetUserFromJwtToken_UserNotFound_Throws()
    {
        var db = CreateDb();
        var jwt = CreateJwt("GhostUser");

        var service = new UserService(db, null);

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.GetUserFromJwtToken(jwt));
    }

    
    
    [Fact]
    public void GetUserFromJwtToken_Valid_ReturnsUser()
    {
        var db = CreateDb();

        db.Users.Add(new Users
        {
            UserId = 1075,
            UserName = "Test User",
            Email = "Test1@test.com",
            PasswordHash = "xx",
            Roles = "Administrator"
        });
        db.SaveChanges();

        var jwt = CreateJwt("Test User");
        var service = new UserService(db, null);

        var result = service.GetUserFromJwtToken(jwt);

        Assert.NotNull(result);
        Assert.Equal("Test User", result.UserName);
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
}