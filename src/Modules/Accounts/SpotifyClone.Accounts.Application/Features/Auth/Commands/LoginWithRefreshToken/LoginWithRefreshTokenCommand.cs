using SpotifyClone.Accounts.Application.Abstractions;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;

public sealed record LoginWithRefreshTokenCommand(
    string RawToken)
    : IAccountsPersistentCommand<LoginWithRefreshTokenCommandResult>;
