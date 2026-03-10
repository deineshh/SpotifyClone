using SpotifyClone.Accounts.Application.Abstractions;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;

public sealed record LoginWithPasswordCommand(
    string Identifier,
    string Password)
    : IAccountsPersistentCommand<LoginWithPasswordCommandResult>;
