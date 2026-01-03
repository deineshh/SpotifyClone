using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Tests.Abstractions.Errors;

public sealed class ErrorTests
{
    [Fact]
    public void Constructor_Should_AssignData()
    {
        // Arrange
        string expectedCode = "Test";
        string expectedDescription = "This error is just for test purposes.";

        // Act
        var error = new Error(expectedCode, expectedDescription);

        // Assert
        error.Code.Should().Be(expectedCode);
    }

    [Fact]
    public void ErrorsWithEqualData_Should_BeEqual()
    {
        // Arrange
        string expectedCode = "Test";
        string expectedDescription = "This error is just for test purposes.";
        var error1 = new Error(expectedCode, expectedDescription);
        var error2 = new Error(expectedCode, expectedDescription);

        // Act
        bool areEqual = error1 == error2;

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ErrorsWithDifferentData_Should_NotBeEqual()
    {
        // Arrange
        var error1 = new Error("Test1", "Test error 1");
        var error2 = new Error("Test2", "Test error 2");

        // Act
        bool areEqual = error1 == error2;

        // Assert
        areEqual.Should().BeFalse();
    }
}
