using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UserService.Data;
using UserService.DTOs;
using UserService.Services;
using Xunit;

namespace UserService.Tests;

public class AuthServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private IConfiguration GetConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "9hkfm5q5Mmm/NfwIochtigQrbVOMBsPjw0mhXvbJv0M=",
                ["Jwt:Issuer"] = "ShopEz",
                ["Jwt:Audience"] = "ShopEzUsers"
            }!)
            .Build();

    [Fact]
    public void Register_ValidUser_ReturnsUser()
    {
        var db = GetInMemoryDb();
        var svc = new AuthService(db, GetConfig());

        var dto = new RegisterDto { Name = "Alice", Email = "alice@test.com", Password = "pass123" };
        var result = svc.Register(dto);

        Assert.NotNull(result);
        Assert.Equal("alice@test.com", result.Email);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public void Register_DuplicateEmail_ThrowsInvalidOperation()
    {
        var db = GetInMemoryDb();
        var svc = new AuthService(db, GetConfig());

        var dto = new RegisterDto { Name = "Alice", Email = "dup@test.com", Password = "pass123" };
        svc.Register(dto);

        Assert.Throws<InvalidOperationException>(() => svc.Register(dto));
    }

    [Fact]
    public void Login_AdminCredentials_ReturnsToken()
    {
        var db = GetInMemoryDb();
        var svc = new AuthService(db, GetConfig());

        var token = svc.Login(new LoginDto { Email = "admin@mail.com", Password = "admin123" });

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Login_InvalidCredentials_ThrowsUnauthorized()
    {
        var db = GetInMemoryDb();
        var svc = new AuthService(db, GetConfig());

        Assert.Throws<UnauthorizedAccessException>(() =>
            svc.Login(new LoginDto { Email = "nobody@test.com", Password = "wrongpass" }));
    }

    [Fact]
    public void Login_ValidUser_ReturnsToken()
    {
        var db = GetInMemoryDb();
        var svc = new AuthService(db, GetConfig());

        svc.Register(new RegisterDto { Name = "Bob", Email = "bob@test.com", Password = "pass123" });
        var token = svc.Login(new LoginDto { Email = "bob@test.com", Password = "pass123" });

        Assert.NotNull(token);
    }
}
