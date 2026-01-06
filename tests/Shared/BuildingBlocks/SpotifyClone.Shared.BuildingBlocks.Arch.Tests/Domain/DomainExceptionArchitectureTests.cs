using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain;

public sealed class DomainExceptionArchitectureTests
{
    [Fact]
    public void DomainExceptionBase_Should_BeAbstractClass()
    {
        // Arrange
        Type domainExceptionType = typeof(DomainExceptionBase);

        // Act
        bool isAbstractClass = domainExceptionType.IsAbstract && domainExceptionType.IsClass;

        // Assert
        isAbstractClass.Should().BeTrue();
    }

    [Fact]
    public void DomainExceptionBase_Should_InheritFrom_Exception()
    {
        // Arrange
        Type domainExceptionType = typeof(DomainExceptionBase);
        Type exceptionType = typeof(Exception);

        // Act
        bool inheritsFromException = exceptionType.IsAssignableFrom(domainExceptionType);

        // Assert
        inheritsFromException.Should().BeTrue();
    }

    // This method will be moved to different test modules once we have some REAL code that implements DomainException.
    [Fact]
    public void DomainExceptions_Should_BeSealed()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainExceptionBase).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainExceptionBase))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    // This method will be moved to different test modules once we have some REAL code that implements DomainException.
    [Fact]
    public void DomainExceptions_Should_HaveDomainExceptionPostfix()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainExceptionBase).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainExceptionBase))
            .Should()
            .HaveNameEndingWith("DomainException")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
