using FluentValidation.TestHelper;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;

namespace SpotifyClone.Accounts.Application.Tests.Features.Auth.Commands.LoginUser;

public sealed class LoginUserValidatorTests
{
    [Fact]
    public void Validate_Should_NotHaveAnyValidationErrors_When_DataIsValid()
    {
        // Arrange
        var validator = new LoginUserValidator();

        // Act
        TestValidationResult<LoginUserCommand> result = validator.TestValidate(
            new LoginUserCommand("example@gmail.com", "StrongPass123"));

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Email_Should_NotBeNullOrEmpty(string? invalidEmail)
    {
        // Arrange
        var validator = new LoginUserValidator();

        // Act
        TestValidationResult<LoginUserCommand> result = validator.TestValidate(
            new LoginUserCommand(invalidEmail!, "StrongPass123!"));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_Should_NotBeNullOrEmpty(string? invalidPassword)
    {
        // Arrange
        var validator = new LoginUserValidator();

        // Act
        TestValidationResult<LoginUserCommand> result = validator.TestValidate(
            new LoginUserCommand("example@gmail.com", invalidPassword!));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
