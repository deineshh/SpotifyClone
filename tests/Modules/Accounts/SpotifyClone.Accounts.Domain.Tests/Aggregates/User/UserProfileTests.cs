using FluentAssertions;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Accounts.Domain.Aggregates.Users.Enums;
using SpotifyClone.Accounts.Domain.Aggregates.Users.ValueObjects;

namespace SpotifyClone.Accounts.Domain.Tests.Aggregates.Users;

public sealed class UserProfileTests
{
    [Fact]
    public void Create_ShouldCreateUser_WhenValidParametersAreProvided()
    {
        // Arrange
        string displayName = "John Doe";
        var birthDate = new DateTimeOffset(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        Gender gender = Gender.Male;
        AvatarImage? avatarImage = null;

        // Act
        var user = UserProfile.Create(displayName, birthDate, gender, avatarImage);

        // Assert
        user.DisplayName.Should().Be(displayName);
        user.BirthDate.Should().Be(birthDate);
        user.AvatarImage.Should().BeNull();
    }
}
