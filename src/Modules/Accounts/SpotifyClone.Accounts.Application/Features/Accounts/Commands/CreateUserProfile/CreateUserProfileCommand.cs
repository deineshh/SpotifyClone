using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    Guid UserId,
    string DisplayName,
    DateTimeOffset BirthDate,
    string Gender)
    : IPersistentCommand<Guid>;
