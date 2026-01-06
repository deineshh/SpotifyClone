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
        string fileType = "png";

        // Act
        var imageMetadata = new ImageMetadata(width, height, fileType);

        // Assert
        Assert.Equal(width, imageMetadata.Width);
        Assert.Equal(height, imageMetadata.Height);
        Assert.Equal("png", imageMetadata.FileType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_ThrowArgumentOutOfRangeException_When_WidthIsInvalid(int invalidWidth)
    {
        // Arrange
        int height = 600;
        string fileType = "JPEG";

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageMetadata(invalidWidth, height, fileType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_ThrowArgumentOutOfRangeException_When_HeightIsInvalid(int invalidHeight)
    {
        // Arrange
        int width = 800;
        string fileType = "JPEG";

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageMetadata(width, invalidHeight, fileType));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_When_FileTypeIsInvalid(string invalidFileType)
    {
        // Arrange
        int width = 800;
        int height = 600;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ImageMetadata(width, height, invalidFileType));
    }

    [Theory]
    [InlineData(" JPEG ", "jpeg")]
    [InlineData("png", "png")]
    [InlineData("  Gif  ", "gif")]
    public void Constructor_Should_NormalizeFileType(string inputFileType, string expectedFileType)
    {
        // Arrange
        int width = 800;
        int height = 600;

        // Act
        var imageMetadata = new ImageMetadata(width, height, inputFileType);

        // Assert
        Assert.Equal(expectedFileType, imageMetadata.FileType);
    }
}
