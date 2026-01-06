using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain;

public sealed class ValueObjectArchitectureTests
{
    [Fact]
    public void ValueObject_Should_BeAbstractClass()
    {
        // Arrange
        Type valueObjectType = typeof(ValueObject);

        // Act
        bool isAbstractClass = valueObjectType.IsAbstract && valueObjectType.IsClass;

        // Assert
        isAbstractClass.Should().BeTrue();
    }

    // This method will be moved to different test modules once we have some REAL code that implements ValueObject.
    [Fact]
    public void ValueObjects_Should_BeSealed()
    {
        // Arrange
        Assembly domainAssembly = typeof(ValueObject).Assembly;

        // Act
        TestResult result = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(ValueObject))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
