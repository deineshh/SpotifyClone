using FluentAssertions;
using Moq;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Accounts.Application.Tests.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>(MockBehavior.Strict);
        _handler = new RegisterUserCommandHandler(_identityMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterUserCommand(
            Email: "existing@test.com",
            Password: "StrongPass123!");

        Error error = AuthErrors.EmailAlreadyInUse;

        _identityMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == error.Code);

        _identityMock.Verify(x =>
            x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CreateUserFails()
    {
        // Arrange
        var command = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "StrongPass123!");

        Error error = AuthErrors.Identity("CreateFailed", "User creation failed");

        _identityMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _identityMock
            .Setup(x => x.CreateUserAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(error));

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == error.Code);

        _identityMock.Verify(x =>
            x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.Verify(x =>
            x.CreateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_ReturnUserId_When_RegistrationSucceeds()
    {
        // Arrange
        var command = new RegisterUserCommand(
            Email: "test@test.com",
            Password: "StrongPass123!");

        var userId = Guid.NewGuid();

        _identityMock
            .Setup(x => x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _identityMock
            .Setup(x => x.CreateUserAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userId));

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userId);

        _identityMock.Verify(x =>
            x.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.Verify(x =>
            x.CreateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.VerifyNoOtherCalls();
    }
}
