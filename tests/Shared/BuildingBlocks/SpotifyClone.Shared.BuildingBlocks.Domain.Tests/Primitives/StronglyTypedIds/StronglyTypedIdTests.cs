using System.Reflection;
using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.StronglyTypedIds;

public sealed class StronglyTypedIdTests
{
    [Fact]
    public void Constructor_Should_AssignValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var testId = new TestId(guid);

        // Assert
        testId.Value.Should().Be(guid);
    }

    [Fact]
    public void EqualValues_Should_BeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var testId1 = new TestId(guid);
        var testId2 = new TestId(guid);

        // Act
        bool areEqual = testId1 == testId2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void NotEqualValues_Should_NotBeEqual()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var testId1 = new TestId(guid1);
        var testId2 = new TestId(guid2);

        // Act
        bool areEqual = testId1 == testId2;

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void EqualValues_Should_HaveSameHashCode()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var testId1 = new TestId(guid);
        var testId2 = new TestId(guid);

        // Assert
        testId1.GetHashCode().Should().Be(testId2.GetHashCode());
    }

    [Fact]
    public void NotEqualValues_Should_HaveDifferentHashCodes()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        // Act
        var testId1 = new TestId(guid1);
        var testId2 = new TestId(guid2);

        // Assert
        testId1.GetHashCode().Should().NotBe(testId2.GetHashCode());
    }

    [Fact]
    public void DifferentTypesWithSameValue_Should_NotBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var testId = new TestId(guid);
        var otherTestId = new OtherTestId(guid);

        // Act
        bool areEqual = testId == otherTestId;

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void DifferentTypesWithSameValue_Should_HaveDifferentHashCodes()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var testId = new TestId(guid);
        var otherTestId = new OtherTestId(guid);

        // Act
        int hashCode1 = testId.GetHashCode();
        int hashCode2 = otherTestId.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
