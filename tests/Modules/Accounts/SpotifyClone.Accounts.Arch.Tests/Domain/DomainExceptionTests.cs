using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Accounts.Domain.Aggregates.User.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Accounts.Arch.Tests.Domain;

public sealed class DomainExceptionTests
{
    private readonly Assembly _domainAssembly = typeof(AvatarImageInvalidShapeDomainException).Assembly;

    [Fact]
    public void DomainExceptions_Should_BeSealed()
    {
        // Arrange & Act
        TestResult result = Types.InAssembly(_domainAssembly)
            .That()
            .Inherit(typeof(DomainExceptionBase))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainExceptions_Should_HaveDomainExceptionPostfix()
    {
        // Arrange & Act
        TestResult result = Types.InAssembly(_domainAssembly)
            .That()
            .Inherit(typeof(DomainExceptionBase))
            .Should()
            .HaveNameEndingWith("DomainException")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
