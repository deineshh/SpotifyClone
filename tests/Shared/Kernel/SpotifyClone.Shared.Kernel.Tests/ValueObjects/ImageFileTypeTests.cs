using FluentAssertions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.Exceptions;
using SpotifyClone.Shared.Kernel.ValueObjects;

namespace SpotifyClone.Shared.Kernel.Tests.ValueObjects;

public sealed class ImageFileTypeTests
{
    [Fact]
    public void ImageFileType_Should_InheritFromValueObject()
    {
        // Arrange & Act
        var imageFileType = ImageFileType.From("png");

        // Assert
        imageFileType.Should().BeAssignableTo<ValueObject>();
    }

    [Theory]
    [InlineData("  PNG ")]
    [InlineData("  JPG ")]
    [InlineData("  JPEG ")]
    [InlineData("  WEBP ")]
    public void From_Should_NormalizeValue(string fileType)
    {
        // Arrange & Act
        var imageFileType = ImageFileType.From(fileType);

        // Assert
        imageFileType.Value.Should().Be(fileType.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("png")]
    [InlineData("jpg")]
    [InlineData("jpeg")]
    [InlineData("webp")]
    public void From_Should_CreateInstance_When_ValueIsSupported(string fileType)
    {
        // Arrange & Act
        Func<ImageFileType> result = () => ImageFileType.From(fileType);

        // Assert
        result.Should().NotThrow<ImageFileTypeNotSupportedDomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void From_Should_ThrowException_When_ValueIsNullOrWhitespace(string invalidFileType)
    {
        // Arrange & Act
        Func<ImageFileType> result = () => ImageFileType.From(invalidFileType);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("gif")]
    public void From_Should_ThrowException_When_ValueIsNotSupported
        (string unsupportedFileType)
    {
        // Arrange & Act
        Func<ImageFileType> result = () => ImageFileType.From(unsupportedFileType);

        // Assert
        result.Should().Throw<ImageFileTypeNotSupportedDomainException>();
    }

    [Fact]
    public void ImageFileType_Should_SupportTransparencyForPngAndWebp()
    {
        // Arrange & Act
        var pngFileType = ImageFileType.From("png");
        var webpFileType = ImageFileType.From("webp");
        var jpgFileType = ImageFileType.From("jpg");
        var jpegFileType = ImageFileType.From("jpeg");

        // Assert
        pngFileType.SupportsTransparency.Should().BeTrue();
        webpFileType.SupportsTransparency.Should().BeTrue();
        jpgFileType.SupportsTransparency.Should().BeFalse();
        jpegFileType.SupportsTransparency.Should().BeFalse();
    }
}
