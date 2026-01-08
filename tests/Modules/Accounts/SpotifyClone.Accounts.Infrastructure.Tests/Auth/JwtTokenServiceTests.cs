using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SpotifyClone.Accounts.Infrastructure.Auth;
using SpotifyClone.Accounts.Infrastructure.Services;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Infrastructure.Tests.Auth;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateAccessToken_Should_CreateValidJwt()
    {
        // Arrange
        IOptions<JwtOptions> options = Options.Create(
            new JwtOptions("test-issuer", "test-audience", "super_super_long_secret_key_123456789", 5));
        var service = new JwtTokenService(options);

        // Act
        string token = service.GenerateAccessToken(
            UserId.From(Guid.NewGuid()),
            "test@email.com",
            new[] { "User" });

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateAccessToken_Should_ContainUserIdAndEmailAndRoles()
    {
        // Arrange
        var userId = UserId.From(Guid.NewGuid());
        IOptions<JwtOptions> options = Options.Create(
            new JwtOptions("test-issuer", "test-audience", "super_super_long_secret_key_123456789", 5));
        var service = new JwtTokenService(options);
        string token = service.GenerateAccessToken(
            userId,
            "user@test.com",
            new[] { "Admin", "User" });

        // Act
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        jwt.Subject.Should().Be(userId.Value.ToString());
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email);
    }

    [Fact]
    public void GenerateAccessToken_Should_SetExpiration()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;
        IOptions<JwtOptions> options = Options.Create(
            new JwtOptions("test-issuer", "test-audience", "super_super_long_secret_key_123456789", 5));
        var service = new JwtTokenService(options);
        string token = service.GenerateAccessToken(
            UserId.From(Guid.NewGuid()),
            "user@test.com",
            Array.Empty<string>());

        // Act
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        jwt.ValidTo.Should().BeAfter(now);
    }
}
