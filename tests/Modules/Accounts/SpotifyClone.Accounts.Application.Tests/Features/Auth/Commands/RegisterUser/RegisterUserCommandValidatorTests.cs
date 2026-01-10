using FluentValidation.TestHelper;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;

namespace SpotifyClone.Accounts.Application.Tests.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandValidatorTests
{
    [Fact]
    public void Validate_Should_NotHaveAnyValidationErrors_When_DataIsValid()
    {
        // Arrange
        var validator = new RegisterUserCommandValidator();

        // Act
        TestValidationResult<RegisterUserCommand> result = validator.TestValidate(
            new RegisterUserCommand("example@gmail.com", "StrongPass123"));

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Email_Should_NotBeNullOrEmpty(string? invalidEmail)
    {
        // Arrange
        var validator = new RegisterUserCommandValidator();

        // Act
        TestValidationResult<RegisterUserCommand> result = validator.TestValidate(
            new RegisterUserCommand(invalidEmail!, "StrongPass123!"));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_Should_NotBeNullOrEmpty(string? invalidPassword)
    {
        // Arrange
        var validator = new RegisterUserCommandValidator();

        // Act
        TestValidationResult<RegisterUserCommand> result = validator.TestValidate(
            new RegisterUserCommand("example@gmail.com", invalidPassword!));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
