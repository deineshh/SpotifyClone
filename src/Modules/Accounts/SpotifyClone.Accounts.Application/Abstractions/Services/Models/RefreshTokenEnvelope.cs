namespace SpotifyClone.Accounts.Application.Abstractions.Services.Models;

public sealed record RefreshTokenEnvelope(
    string RawToken,
    DateTimeOffset ExpiresAt);
