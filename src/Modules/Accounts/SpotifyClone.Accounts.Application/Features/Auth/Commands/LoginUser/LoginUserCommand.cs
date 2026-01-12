using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password)
    : IPersistentCommand<LoginUserResult>;
