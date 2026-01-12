using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Abstractions.Services.Models;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;

internal sealed class LoginUserCommandHandler(
    IAccountsUnitOfWork unit,
    IIdentityService identity,
    ITokenService tokenService,
    ITokenHasher tokenHasher)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IAccountsUnitOfWork _unit = unit;
    private readonly IIdentityService _identity = identity;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ITokenHasher _tokenHasher = tokenHasher;

    public async Task<Result<LoginUserResult>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        Result<IdentityUserInfo> identityResult = await _identity.ValidateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (identityResult.IsFailure)
        {
            return Result.Failure<LoginUserResult>(identityResult.Errors);
        }

        UserId userId = identityResult.Value.UserId;
        string email = identityResult.Value.Email;

        AccessToken accessToken = _tokenService.GenerateAccessToken(userId, email, ["User"]);
        RefreshTokenEnvelope refreshToken = _tokenService.GenerateRefreshToken();
        string refreshTokenHash = _tokenHasher.Hash(refreshToken.RawToken);

        Result storeResult = await _unit.RefreshTokens.StoreAsync(
            userId, refreshTokenHash, refreshToken.ExpiresAt, cancellationToken);

        if (storeResult.IsFailure)
        {
            return Result.Failure<LoginUserResult>(storeResult.Errors);
        }

        return Result.Success(new LoginUserResult(
            userId.Value,
            accessToken.RawToken,
            accessToken.ExpiresAt,
            refreshToken.RawToken));
    }
}
