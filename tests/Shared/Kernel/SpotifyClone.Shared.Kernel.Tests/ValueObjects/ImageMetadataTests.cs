using FluentAssertions;
using SpotifyClone.Shared.Kernel.Enums;
using SpotifyClone.Shared.Kernel.Exceptions;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Shared.Kernel.Tests.ValueObjects;

public sealed class ImageMetadataTests
{
    [Fact]
    public void Constructor_Should_CreateInstance_When_ParametersAreValid()
    {
        // Arrange
        int width = 800;
        int height = 600;
        ImageFileType fileType = ImageFileType.Jpg;
        long sizeInBytes = 150000;

        // Act
        var imageMetadata = new ImageMetadata(width, height, fileType, sizeInBytes);

        // Assert
        imageMetadata.Width.Should().Be(width);
        imageMetadata.Height.Should().Be(height);
        imageMetadata.FileType.Should().Be(ImageFileType.Jpg);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_ThrowException_When_WidthIsInvalid(int invalidWidth)
    {
        // Arrange
        int height = 600;
        ImageFileType fileType = ImageFileType.Jpg;
        long sizeInBytes = 150000;

        // Act
        Func<ImageMetadata> result = ()
            => new ImageMetadata(invalidWidth, height, fileType, sizeInBytes);

        // Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_ThrowException_When_HeightIsInvalid(int invalidHeight)
    {
        // Arrange
        int width = 600;
        ImageFileType fileType = ImageFileType.Jpg;
        long sizeInBytes = 150000;

        // Act
        Func<ImageMetadata> result = ()
            => new ImageMetadata(width, invalidHeight, fileType, sizeInBytes);

        // Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_FileTypeIsNull()
    {
        // Arrange
        int width = 600;
        int height = 600;
        long sizeInBytes = 150000;

        // Act
        Func<ImageMetadata> result = ()
            => new ImageMetadata(width, height, null!, sizeInBytes);

        // Assert
        result.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_ThrowException_When_SizeInBytesIsInvalid(int invalidSizeInBytes)
    {
        // Arrange
        int width = 600;
        int height = 600;
        ImageFileType fileType = ImageFileType.Jpg;

        // Act
        Func<ImageMetadata> result = ()
            => new ImageMetadata(width, height, fileType, invalidSizeInBytes);

        // Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }
}
