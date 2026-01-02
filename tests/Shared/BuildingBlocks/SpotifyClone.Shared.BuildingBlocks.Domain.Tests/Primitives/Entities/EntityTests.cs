using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.StronglyTypedIds;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.Entities;

public sealed class EntityTests
{
    [Fact]
    public void EntitiesWithSameId_Should_BeEqual()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id, "Entity One");
        var entity2 = new TestEntity(id, "Entity Two");

        // Act
        bool areEqual = entity1 == entity2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void EntitiesWithDifferentIds_Should_NotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(new TestId(Guid.NewGuid()), "Entity One");
        var entity2 = new TestEntity(new TestId(Guid.NewGuid()), "Entity Two");

        // Act
        bool areEqual = entity1 == entity2;

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void EntitiesWithSameId_Should_HaveSameHashCode()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id, "Entity One");
        var entity2 = new TestEntity(id, "Entity Two");

        // Act
        int hashCode1 = entity1.GetHashCode();
        int hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void EntitiesWithDifferentIds_Should_HaveDifferentHashCodes()
    {
        // Arrange
        var entity1 = new TestEntity(new TestId(Guid.NewGuid()), "Entity One");
        var entity2 = new TestEntity(new TestId(Guid.NewGuid()), "Entity Two");

        // Act
        int hashCode1 = entity1.GetHashCode();
        int hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void EntitiesOfDifferentTypesWithSameId_Should_NotBeEqual()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id, "Test Entity");
        var entity2 = new OtherTestEntity(new OtherTestId(id.Value), "Other Test Entity");

        // Act
        bool areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void EntityWithNullId_Should_ThrowException()
    {
        // Arrange, Act
        Action act = () => new TestEntity(null!, "Entity with Null Id");

        // Assert
        act.Should().Throw<IdNullDomainException>();
    }
}
