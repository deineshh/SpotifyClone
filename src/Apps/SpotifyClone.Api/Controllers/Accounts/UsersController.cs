using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Accounts.Application.Features.Accounts.Commands.Delete;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries.GetAllByOwner;
using SpotifyClone.Shared.BuildingBlocks.Application.Auth;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Accounts;

[Route("api/v1/users")]
public sealed class UsersController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Get Playlists by User")]
    [EndpointDescription("Returns Playlists owned by a certain User.")]
    [ProducesResponseType(typeof(PlaylistList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    [HttpGet("{id:guid}/playlists")]
    public async Task<ActionResult<PlaylistList>> GetByOwner(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<PlaylistList> result = await Mediator.Send(
            new GetAllPlaylistsByOwnerQuery(id),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok(result.Value);
    }

    [EndpointSummary("Delete User")]
    [EndpointDescription("Deletes a User and his entire profile.")]
    [ProducesResponseType(typeof(PlaylistList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = UserRoles.Listener)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<DeleteUserCommandResult> result = await Mediator.Send(
            new DeleteUserCommand(id),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok();
    }
}
