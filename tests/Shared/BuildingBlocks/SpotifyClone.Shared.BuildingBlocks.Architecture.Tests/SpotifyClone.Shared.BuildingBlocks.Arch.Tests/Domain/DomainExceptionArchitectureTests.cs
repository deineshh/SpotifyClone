using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain;

public sealed class DomainExceptionArchitectureTests
{
    [Fact]
    public void DomainException_Should_BeAbstractClass()
    {
        // Arrange
        Type domainExceptionType = typeof(DomainException);

        // Act
        bool isAbstractClass = domainExceptionType.IsAbstract && domainExceptionType.IsClass;

        // Assert
        isAbstractClass.Should().BeTrue();
    }

    [Fact]
    public void DomainException_Should_InheritFrom_Exception()
    {
        // Arrange
        Type domainExceptionType = typeof(DomainException);
        Type exceptionType = typeof(Exception);

        // Act
        bool inheritsFromException = exceptionType.IsAssignableFrom(domainExceptionType);

        // Assert
        inheritsFromException.Should().BeTrue();
    }

    [Fact]
    public void DomainExceptions_Should_BeSealed()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainException).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainException))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainExceptions_Should_HaveDomainExceptionPostfix()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainException).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainException))
            .Should()
            .HaveNameEndingWith("DomainException")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
