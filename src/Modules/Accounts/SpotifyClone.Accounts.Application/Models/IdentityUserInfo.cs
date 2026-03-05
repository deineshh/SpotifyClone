using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Models;

public sealed record IdentityUserInfo(
    UserId UserId,
    string Email,
    bool EmailConfirmed,
    bool RequiresTwoFactor);
