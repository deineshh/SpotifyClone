using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Arch.Tests.Domain.Exceptions;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.Exceptions;

public sealed class DomainExceptionTests
{
    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_MessageIsNull()
    {
        // Arrange & Act
        Action actNull = () => new NullTestDomainException();

        // Assert
        actNull.Should().Throw<ArgumentException>().WithMessage("Domain exception message is required.");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_MessageIsEmpty()
    {
        // Arrange & Act
        Action actEmpty = () => new EmptyTestDomainException();

        // Assert
        actEmpty.Should().Throw<ArgumentException>().WithMessage("Domain exception message is required.");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_MessageIsWhitespace()
    {
        // Arrange & Act
        Action actWhitespace = () => new WhitespaceTestDomainException();

        // Assert
        actWhitespace.Should().Throw<ArgumentException>().WithMessage("Domain exception message is required.");
    }
}
