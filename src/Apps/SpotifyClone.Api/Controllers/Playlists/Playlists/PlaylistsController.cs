using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Playlists.Playlists.Create;
using SpotifyClone.Api.Contracts.v1.Playlists.Playlists.LinkNewCover;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Playlists.Application.Features.Playlists.Commands.Create;
using SpotifyClone.Playlists.Application.Features.Playlists.Commands.LinkNewCover;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries.GetAllByOwner;
using SpotifyClone.Playlists.Application.Features.Playlists.Queries.GetDetails;
using SpotifyClone.Shared.BuildingBlocks.Application.Auth;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Playlists.Playlists;

[Tags("Playlists Module")]
[Route("api/v1/playlists")]
public sealed class PlaylistsController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Get Playlist Details")]
    [EndpointDescription("Returns all the necessary Playlist details.")]
    [ProducesResponseType(typeof(PlaylistDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlaylistDetails>> GetDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<PlaylistDetails> result = await Mediator.Send(
            new GetPlaylistDetailsQuery(id),
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

    [EndpointSummary("Get all Playlists by Owner")]
    [EndpointDescription("Returns Playlists owned by a certain User.")]
    [ProducesResponseType(typeof(PlaylistList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    [HttpGet("users/{ownerId:guid}/playlists")]
    public async Task<ActionResult<PlaylistList>> GetByOwner(
        [FromQuery] Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        Result<PlaylistList> result = await Mediator.Send(
            new GetAllPlaylistsByOwnerQuery(ownerId),
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

    [EndpointSummary("Create Playlist")]
    [EndpointDescription("Creates a private Playlist.")]
    [ProducesResponseType(typeof(CreatePlaylistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = UserRoles.Listener)]
    [HttpPost]
    public async Task<ActionResult<CreatePlaylistResponse>> CreatePlaylist(
        CancellationToken cancellationToken = default)
    {
        Result<CreatePlaylistCommandResult> createResult = await Mediator.Send(
            new CreatePlaylistCommand(),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return CreatedAtAction(nameof(GetDetails),
            new { id = createResult.Value.PlaylistId },
            new CreatePlaylistResponse(
                createResult.Value.PlaylistId));
    }

    [EndpointSummary("Link Playlist to new cover image")]
    [EndpointDescription("Links a Playlist to a new cover image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = UserRoles.Listener)]
    [HttpPut("{id:guid}/cover")]
    public async Task<ActionResult> LinkNewCoverImage(
        [FromRoute] Guid id,
        [FromBody] LinkNewCoverToPlaylistRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewCoverToPlaylistCommandResult> result = await Mediator.Send(
            new LinkNewCoverToPlaylistCommand(
                id,
                request.ImageId,
                request.ImageWidth,
                request.ImageHeight,
                request.ImageFileType,
                request.ImageSizeInBytes),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return NoContent();
    }
}
