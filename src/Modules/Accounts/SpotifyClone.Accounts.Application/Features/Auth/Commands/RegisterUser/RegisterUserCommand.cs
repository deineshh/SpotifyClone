using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password)
    : IPersistentCommand<Guid>;
