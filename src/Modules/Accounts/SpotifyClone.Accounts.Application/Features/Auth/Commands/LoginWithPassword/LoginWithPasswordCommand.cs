using SpotifyClone.Accounts.Application.Abstractions;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;

public sealed record LoginWithPasswordCommand(
    string Email,
    string Password)
    : IAccountsPersistentCommand<LoginWithPasswordCommandResult>;
