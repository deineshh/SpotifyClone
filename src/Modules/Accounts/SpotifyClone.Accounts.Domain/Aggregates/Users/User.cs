using SpotifyClone.Accounts.Domain.Aggregates.Users.Enums;
using SpotifyClone.Accounts.Domain.Aggregates.Users.Rules;
using SpotifyClone.Accounts.Domain.Aggregates.Users.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Domain.Aggregates.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    public string DisplayName { get; private set; }
    public DateTimeOffset BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public AvatarImage? AvatarImage { get; private set; }

    private User(UserId id, string displayName, DateTimeOffset birthDate, Gender gender, AvatarImage? avatarImage)
        : base(id)
    {
        DisplayName = displayName;
        BirthDate = birthDate;
        Gender = gender;
        AvatarImage = avatarImage;
    }

    public static User Create(
        UserId id, string displayName, DateTimeOffset birthDate, Gender gender, AvatarImage? avatarImage)
    {
        DisplayNameRules.Validate(displayName);
        BirthDateRules.Validate(birthDate);

        return new User(id, displayName, birthDate, gender, avatarImage);
    }
}
