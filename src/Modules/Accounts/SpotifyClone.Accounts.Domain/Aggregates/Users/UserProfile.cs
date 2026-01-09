using SpotifyClone.Accounts.Domain.Aggregates.Users.Enums;
using SpotifyClone.Accounts.Domain.Aggregates.Users.Rules;
using SpotifyClone.Accounts.Domain.Aggregates.Users.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users;

public sealed class UserProfile : AggregateRoot<UserId, Guid>
{
    public string DisplayName { get; private set; }
    public DateTimeOffset BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public AvatarImage? AvatarImage { get; private set; }

    private UserProfile(
        UserId id, string displayName, DateTimeOffset birthDate, Gender gender, AvatarImage? avatarImage)
        : base(id)
    {
        DisplayName = displayName;
        BirthDate = birthDate;
        Gender = gender;
        AvatarImage = avatarImage;
    }

    public static UserProfile Create(
        string displayName, DateTimeOffset birthDate, Gender gender, AvatarImage? avatarImage)
    {
        DisplayNameRules.Validate(displayName);
        BirthDateRules.Validate(birthDate);

        return new UserProfile(UserId.New(), displayName, birthDate, gender, avatarImage);
    }
}
