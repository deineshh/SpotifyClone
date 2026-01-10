using FluentAssertions;
using Moq;
using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Accounts.Domain.Aggregates.Users.Enums;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Tests.Features.Accounts.Commands.CreateUserProfile;

public sealed class CreateUserProfileCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock = new(MockBehavior.Strict);
    private readonly Mock<IUserProfileRepository> _profilesMock = new(MockBehavior.Strict);
    private readonly Mock<IAccountsUnitOfWork> _unitMock = new(MockBehavior.Strict);

    private readonly CreateUserProfileCommandHandler _handler;

    public CreateUserProfileCommandHandlerTests()
    {
        _unitMock
            .SetupGet(x => x.UserProfiles)
            .Returns(_profilesMock.Object);

        _handler = new CreateUserProfileCommandHandler(
            _identityMock.Object,
            _unitMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_IdentityUserDoesNotExist()
    {
        // Arrange
        var command = new CreateUserProfileCommand(
            UserId: Guid.NewGuid(),
            DisplayName: "John",
            BirthDate: DateTimeOffset.UtcNow.AddYears(-20),
            Gender: "Male");

        _identityMock
            .Setup(x => x.UserExistsAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == AuthErrors.UserNotFound);

        _identityMock.VerifyAll();
        _profilesMock.VerifyNoOtherCalls();
        _unitMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserProfileAlreadyExists()
    {
        // Arrange
        var userGuid = Guid.NewGuid();
        var userId = UserId.From(userGuid);

        var command = new CreateUserProfileCommand(
            UserId: userGuid,
            DisplayName: "John",
            BirthDate: DateTimeOffset.UtcNow.AddYears(-20),
            Gender: "Male");

        _identityMock
            .Setup(x => x.UserExistsAsync(userGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var userProfile = UserProfile.Create(
            id: userId,
            displayName: "John",
            birthDate: DateTimeOffset.UtcNow.AddYears(-20),
            gender: Gender.From("Male"));

        _profilesMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == UserProfileErrors.AlreadyExists);

        _identityMock.VerifyAll();
        _profilesMock.VerifyAll();
        _unitMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_CreateUserProfile_When_DataIsValid()
    {
        // Arrange
        var userGuid = Guid.NewGuid();
        var userId = UserId.From(userGuid);

        var command = new CreateUserProfileCommand(
            UserId: userGuid,
            DisplayName: "John",
            BirthDate: DateTimeOffset.UtcNow.AddYears(-20),
            Gender: "Male");

        _identityMock
            .Setup(x => x.UserExistsAsync(userGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _profilesMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        _profilesMock
            .Setup(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _profilesMock.Verify(x =>
            x.AddAsync(
                It.Is<UserProfile>(p =>
                    p.DisplayName == "John" &&
                    p.Gender == Gender.Male),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _identityMock.VerifyAll();
        _profilesMock.VerifyAll();
    }
}
