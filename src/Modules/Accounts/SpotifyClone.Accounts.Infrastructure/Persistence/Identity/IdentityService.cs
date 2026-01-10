using Microsoft.AspNetCore.Identity;
using SpotifyClone.Accounts.Application.Abstractions.Services;
using SpotifyClone.Accounts.Application.Abstractions.Services.Models;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Accounts.Infrastructure.Persistence.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result<IdentityUserInfo>> ValidateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<IdentityUserInfo>(
                AuthErrors.InvalidEmail);
        }

        if (!await _signInManager.CanSignInAsync(user))
        {
            return Result.Failure<IdentityUserInfo>(
                AuthErrors.SignInNotAllowed);
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            return Result.Failure<IdentityUserInfo>(
                AuthErrors.InvalidPassword);
        }

        bool requiresTwoFactor = await _userManager.GetTwoFactorEnabledAsync(user);

        var userInfo = new IdentityUserInfo(
            UserId.From(user.Id),
            user.Email!,
            user.EmailConfirmed,
            requiresTwoFactor);

        return Result.Success<IdentityUserInfo>(userInfo);
    }

    public async Task<Result<IReadOnlyCollection<string>>> GetUserRolesAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.Value.ToString());

        if (user is null)
        {
            return Result.Failure<IReadOnlyCollection<string>>(AuthErrors.UserNotFound);
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);

        return roles.AsReadOnly();
    }

    public async Task<Result<bool>> IsTwoFactorEnabledAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId.Value.ToString());

        if (user is null)
        {
            return Result.Failure<bool>(AuthErrors.UserNotFound);
        }

        return await _userManager.GetTwoFactorEnabledAsync(user);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return true;
        }

        return false;
    }

    public async Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        IdentityResult result =
            await _userManager.CreateAsync((ApplicationUser)user, password);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(
                result.Errors.Select(e =>
                    AuthErrors.Identity(e.Code, e.Description)).ToArray());
        }

        return Result.Success(user.Id);
    }
}
