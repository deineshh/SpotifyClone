using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IIdentityService identity)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IIdentityService _identity = identity;

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        bool emailExists = await _identity.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<Guid>(AuthErrors.EmailAlreadyInUse);
        }

        Result<Guid> identityResult = await _identity.CreateUserAsync(
            request.Email, request.Password, cancellationToken);

        if (identityResult.IsFailure)
        {
            return identityResult;
        }

        return identityResult.Value;
    }
}
