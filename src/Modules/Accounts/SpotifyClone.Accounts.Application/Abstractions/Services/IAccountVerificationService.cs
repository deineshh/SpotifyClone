using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Accounts.Application.Abstractions.Services;

public interface IAccountVerificationService
{
    Task<Result> SendEmailChangedEmailAsync(
        string oldEmail,
        string newEmail,
        string displayName,
        CancellationToken cancellationToken = default);

    Task<Result> SendVerificationEmailAsync(
        string email,
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    Task<Result> SendPhoneNumberVerificationAsync(
        Guid userId,
        string phoneNumber);

    Task<Result> VerifyPhoneNumberAsync(
        Guid userId,
        string phoneNumber,
        string token,
        CancellationToken cancellationToken = default);
}
