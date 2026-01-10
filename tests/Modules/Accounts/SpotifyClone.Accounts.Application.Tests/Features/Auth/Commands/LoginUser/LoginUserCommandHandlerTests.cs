using FluentAssertions;
using Moq;
using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Abstractions.Services.Models;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Tests.Features.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IAccountsUnitOfWork> _unitMock = new();
    private readonly Mock<ITokenHasher> _tokenHasherMock = new();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
        => _handler = new LoginUserCommandHandler(
            _unitMock.Object,
            _identityMock.Object,
            _tokenServiceMock.Object,
            _tokenHasherMock.Object);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_IdentityValidationFails()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "Password123!");
        Error error = AuthErrors.InvalidEmail;

        _identityMock
            .Setup(x => x.ValidateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IdentityUserInfo>(error));

        // Act
        Result<LoginUserResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == error);
        _identityMock.Verify(
            x => x.ValidateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RefreshTokenStoreFails()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "Password123!");
        var userId = UserId.New();
        var identityInfo = new IdentityUserInfo(userId, "test@test.com", true, false);

        _identityMock
            .Setup(x => x.ValidateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(identityInfo));

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(userId, identityInfo.Email, It.IsAny<IReadOnlyCollection<string>>()))
            .Returns(new AccessToken("accessToken", DateTimeOffset.UtcNow.AddMinutes(5)));

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(new RefreshTokenEnvelope("rawRefreshToken", DateTimeOffset.UtcNow.AddDays(30)));

        _tokenHasherMock
            .Setup(x => x.Hash("rawRefreshToken"))
            .Returns("hashedRefreshToken");

        Error error = RefreshTokenErrors.InvalidToken;

        _unitMock
            .Setup(x => x.RefreshTokens.StoreAsync(
                userId,
                "hashedRefreshToken",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        // Act
        Result<LoginUserResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == error);

        _unitMock.Verify(
            x => x.RefreshTokens.StoreAsync(
                userId, "hashedRefreshToken",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_LoginSucceeds()
    {
        // Arrange
        var command = new LoginUserCommand("test@test.com", "Password123!");
        var userId = UserId.New();
        var identityInfo = new IdentityUserInfo(userId, "test@test.com", true, false);

        _identityMock
            .Setup(x => x.ValidateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(identityInfo));

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(userId, identityInfo.Email, It.IsAny<IReadOnlyCollection<string>>()))
            .Returns(new AccessToken("accessToken", DateTimeOffset.UtcNow));

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(new RefreshTokenEnvelope("rawRefreshToken123", DateTimeOffset.UtcNow.AddDays(7)));

        _tokenHasherMock
            .Setup(x => x.Hash("rawRefreshToken123"))
            .Returns("hashedRefreshToken123");

        _unitMock
            .Setup(x => x.RefreshTokens.StoreAsync(
                userId,
                "hashedRefreshToken123",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _unitMock
            .Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(1));

        // Act
        Result<LoginUserResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("accessToken");
        result.Value.RefreshToken.Should().Be("rawRefreshToken123");
        result.Value.UserId.Should().Be(userId.Value);

        _identityMock.Verify(
            x => x.ValidateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitMock.Verify(
            x => x.RefreshTokens.StoreAsync(
                userId, "hashedRefreshToken123",
                It.IsAny<DateTimeOffset>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
