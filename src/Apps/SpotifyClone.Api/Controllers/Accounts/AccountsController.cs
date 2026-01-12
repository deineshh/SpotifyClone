using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Accounts.Application.Features.Accounts.Commands.CreateUserProfile;
using SpotifyClone.Api.Contracts.v1.Accounts.Profile.CreateUserProfile;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Accounts;

[Route("api/accounts")]
public sealed class AccountsController : ApiController
{
    public AccountsController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost("profile")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUserProfile(
        CreateUserProfileRequest request,
        CancellationToken ct)
    {
        Result<Guid> result = await Mediator.Send(
            new CreateUserProfileCommand(
                request.UserId,
                request.DisplayName,
                request.BirthDate,
                request.Gender),
            ct);

        if (result.IsFailure)
        {
            return Conflict(result.Errors);
        }

        return Created(
            uri: new Uri($"/api/accounts/profile/{result.Value}"),
            value: new CreateUserProfileResponse(
                result.Value));
    }
}
