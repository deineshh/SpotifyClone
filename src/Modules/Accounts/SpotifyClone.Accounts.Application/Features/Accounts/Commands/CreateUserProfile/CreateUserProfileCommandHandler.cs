using SpotifyClone.Accounts.Application.Abstractions;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Domain.Aggregates.Users;
using SpotifyClone.Accounts.Domain.Aggregates.Users.Enums;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;

internal sealed class CreateUserProfileCommandHandler(
    IIdentityService identityService,
    IAccountsUnitOfWork unit)
    : ICommandHandler<CreateUserProfileCommand, Guid>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IAccountsUnitOfWork _unit = unit;

    public async Task<Result<Guid>> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await _identityService.UserExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
        {
            return Result.Failure<Guid>(AuthErrors.UserNotFound);
        }

        var userId = UserId.From(request.UserId);

        UserProfile? existingUserProfile = await _unit.UserProfiles.GetByUserIdAsync(userId, cancellationToken);
        if (existingUserProfile is not null)
        {
            return Result.Failure<Guid>(UserProfileErrors.AlreadyExists);
        }

        var userProfile = UserProfile.Create(
            userId,
            request.DisplayName,
            request.BirthDate,
            Gender.From(request.Gender));

        await _unit.UserProfiles.AddAsync(userProfile, cancellationToken);

        return userProfile.Id.Value;
    }
}
