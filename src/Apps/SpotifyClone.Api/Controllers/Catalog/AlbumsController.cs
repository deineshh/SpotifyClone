using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.CorrectTitle;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.LinkNewCover;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.MoveTrack;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.PublishAlbum;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.RescheduleRelease;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.AddTrack;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.CorrectTitle;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.Delete;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.LinkNewCover;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.MoveTrack;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.PublishAlbum;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.RemoveTrack;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.RescheduleRelease;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.UnpublishAlbum;
using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Application.Features.Albums.Queries.GetDetails;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Catalog;

[Tags("Catalog Module")]
[Route("api/v1/albums")]
public sealed class AlbumsController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Create Album")]
    [EndpointDescription("Creates an Album in a Draft state.")]
    [ProducesResponseType(typeof(CreateAlbumResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<CreateAlbumResponse>> CreateAlbum(
        [FromBody] CreateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CreateAlbumCommandResult> createResult = await Mediator.Send(
            new CreateAlbumCommand(
                request.Title,
                request.MainArtists),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateAlbumCommandResult createResultData = createResult.Value;

        return CreatedAtAction(nameof(GetAlbumDetails),
            new { id = createResultData.AlbumId },
            new CreateAlbumResponse(
                createResultData.AlbumId));
    }

    [EndpointSummary("Get Album Details")]
    [EndpointDescription("Returns all the necessary Album details.")]
    [ProducesResponseType(typeof(AlbumDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlbumDetailsResponse>> GetAlbumDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<AlbumDetailsResponse> result = await Mediator.Send(
            new GetAlbumDetailsQuery(id),
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

    [EndpointSummary("Delete Album")]
    [EndpointDescription("Deletes an Album if it's not yet published.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAlbum(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<DeleteAlbumCommandResult> result = await Mediator.Send(
            new DeleteAlbumCommand(id),
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

    [EndpointSummary("Link Album to new Cover image")]
    [EndpointDescription("Links an Album to a new Cover image if it's not yet published.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/cover")]
    public async Task<ActionResult> LinkNewCoverImage(
        [FromRoute] Guid id,
        [FromBody] LinkAlbumToNewCoverRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewCoverToAlbumCommandResult> result = await Mediator.Send(
            new LinkNewCoverToAlbumCommand(
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

    [EndpointSummary("Publish Album")]
    [EndpointDescription("Publishes an Album and all of it's Tracks if the Album is ready to publish.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult> PublishAlbum(
        [FromRoute] Guid id,
        [FromBody] PublishAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<PublishAlbumCommandResult> result = await Mediator.Send(
            new PublishAlbumCommand(
                id,
                request.ReleaseDate),
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

    [EndpointSummary("Unpublish Album")]
    [EndpointDescription("Unpublishes an Album and all of it's Tracks if the Album is published.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/unpublish")]
    public async Task<ActionResult> UnpublishAlbum(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnpublishAlbumCommandResult> result = await Mediator.Send(
            new UnpublishAlbumCommand(id),
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

    [EndpointSummary("Add Track to Album")]
    [EndpointDescription("Adds a new Track to an Album if the Track is not yet attached to a different album.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/tracks/{trackId:guid}")]
    public async Task<ActionResult> AddTrackToAlbum(
        [FromRoute] Guid id,
        [FromRoute] Guid trackId,
        CancellationToken cancellationToken = default)
    {
        Result<AddTrackToAlbumCommandResult> result = await Mediator.Send(
            new AddTrackToAlbumCommand(id, trackId),
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

    [EndpointSummary("Remove Track from Album")]
    [EndpointDescription("Removes an existing Track from an Album and archive the removed Track.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/tracks/{trackId:guid}")]
    public async Task<ActionResult> RemoveTrackFromAlbum(
        [FromRoute] Guid id,
        [FromRoute] Guid trackId,
        CancellationToken cancellationToken = default)
    {
        Result<RemoveTrackFromAlbumCommandResult> result = await Mediator.Send(
            new RemoveTrackFromAlbumCommand(id, trackId),
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

    [EndpointSummary("Move Track in Album")]
    [EndpointDescription("Move an existing Track in an Album.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/tracks/{trackId:guid}/move")]
    public async Task<ActionResult> MoveTrackInAlbum(
        [FromRoute] Guid id,
        [FromRoute] Guid trackId,
        [FromBody] MoveTrackInAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<MoveTrackInAlbumCommandResult> result = await Mediator.Send(
            new MoveTrackInAlbumCommand(
                id, trackId, request.TargetPositionIndex),
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

    [EndpointSummary("Correct Album title")]
    [EndpointDescription("Corrects the Album title.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPatch("{id:guid}/title")]
    public async Task<ActionResult> CorrectAlbumTitle(
        [FromRoute] Guid id,
        [FromBody] CorrectAlbumTitleRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CorrectAlbumTitleCommandResult> result = await Mediator.Send(
            new CorrectAlbumTitleCommand(
                id,
                request.Title),
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

    [EndpointSummary("Reschedule Album Release")]
    [EndpointDescription("Reschedules Album release if it's published but not released yet. " +
        "Automatically does the same for all it's Tracks.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/release")]
    public async Task<ActionResult> RescheduleAlbumRelease(
        [FromRoute] Guid id,
        [FromBody] RescheduleAlbumReleaseRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<RescheduleAlbumReleaseCommandResult> result = await Mediator.Send(
            new RescheduleAlbumReleaseCommand(
                id,
                request.ReleaseDate),
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
