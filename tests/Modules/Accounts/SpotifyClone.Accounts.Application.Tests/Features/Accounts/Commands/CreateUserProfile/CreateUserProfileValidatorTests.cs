using FluentValidation.TestHelper;
using SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;

namespace SpotifyClone.Accounts.Application.Tests.Features.Accounts.Commands.CreateUserProfile;

public sealed class CreateUserProfileValidatorTests
{
    [Fact]
    public void Validate_Should_NotHaveAnyValidationErrors_When_DataIsValid()
    {
        // Arrange
        var validator = new CreateUserProfileValidator();

        // Act
        TestValidationResult<CreateUserProfileCommand> result = validator.TestValidate(
            new CreateUserProfileCommand(Guid.NewGuid(), "Name", DateTimeOffset.UtcNow, "Male"));

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DisplayName_Should_NotBeNullOrEmpty(string? invalidDisplayName)
    {
        // Arrange
        var validator = new CreateUserProfileValidator();

        // Act
        TestValidationResult<CreateUserProfileCommand> result = validator.TestValidate(
            new CreateUserProfileCommand(Guid.NewGuid(), invalidDisplayName!, DateTimeOffset.UtcNow, "Male"));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Gender_Should_NotBeNullOrEmpty(string? invalidGender)
    {
        // Arrange
        var validator = new CreateUserProfileValidator();

        // Act
        TestValidationResult<CreateUserProfileCommand> result = validator.TestValidate(
            new CreateUserProfileCommand(Guid.NewGuid(), "Name", DateTimeOffset.UtcNow, invalidGender!));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }
}
