using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain;

public sealed class DomainEventArchitectureTests
{
    [Fact]
    public void DomainEvents_Should_BeAbstractClass()
    {
        // Arrange
        Type domainEventType = typeof(DomainEvent);

        // Act
        bool isAbstractClass = domainEventType.IsAbstract && domainEventType.IsClass;

        // Assert
        isAbstractClass.Should().BeTrue();
    }

    // This method will be moved to a different test module once we have some REAL code that implements domain events.
    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainEvent).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    // This method will be moved to a different test module once we have some REAL code that implements domain events.
    [Fact]
    public void DomainEvents_Should_HaveDomainEventPostfix()
    {
        // Arrange
        Assembly domainAssembly = typeof(DomainEvent).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
