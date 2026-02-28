using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Moods.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Moods.LinkNewCover;
using SpotifyClone.Api.Contracts.v1.Catalog.Moods.Rename;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Moods.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Moods.Commands.Delete;
using SpotifyClone.Catalog.Application.Features.Moods.Commands.LinkNewCover;
using SpotifyClone.Catalog.Application.Features.Moods.Commands.Rename;
using SpotifyClone.Catalog.Application.Features.Moods.Commands.UnlinkCover;
using SpotifyClone.Catalog.Application.Features.Moods.Queries;
using SpotifyClone.Catalog.Application.Features.Moods.Queries.GetDetails;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByMood;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Catalog;

[Tags("Catalog Module")]
[Route("api/v1/moods")]
public sealed class MoodsController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Create Mood")]
    [EndpointDescription("Creates a Mood without cover.")]
    [ProducesResponseType(typeof(CreateMoodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<CreateMoodResponse>> CreateMood(
        [FromBody] CreateMoodRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CreateMoodCommandResult> createResult = await Mediator.Send(
            new CreateMoodCommand(request.Name),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateMoodCommandResult createResultData = createResult.Value;

        return CreatedAtAction(nameof(GetMoodDetails),
            new { id = createResultData.MoodId },
            new CreateMoodResponse(
                createResultData.MoodId));
    }

    [EndpointSummary("Get Mood Details")]
    [EndpointDescription("Returns all the necessary Mood details.")]
    [ProducesResponseType(typeof(MoodDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MoodDetails>> GetMoodDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<MoodDetails> result = await Mediator.Send(
            new GetMoodDetailsQuery(id),
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

    [EndpointSummary("Get all Tracks by a Mood")]
    [EndpointDescription("Returns all Tracks by a specific Mood.")]
    [ProducesResponseType(typeof(TrackList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}/tracks")]
    public async Task<ActionResult<TrackList>> GetAllTracksByMoodDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<TrackList> result = await Mediator.Send(
            new GetAllTracksByMoodQuery(id),
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

    [EndpointSummary("Link Mood to new cover image")]
    [EndpointDescription("Links a Mood to a new cover image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/cover")]
    public async Task<ActionResult> LinkNewCoverImage(
        [FromRoute] Guid id,
        [FromBody] LinkNewCoverToMoodRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewCoverToMoodCommandResult> result = await Mediator.Send(
            new LinkNewCoverToMoodCommand(
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

    [EndpointSummary("Unlink Mood from cover image")]
    [EndpointDescription("Unlinks a Mood from it's cover image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/cover")]
    public async Task<ActionResult> UnlinkCoverImage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnlinkCoverFromMoodCommandResult> result = await Mediator.Send(
            new UnlinkCoverFromMoodCommand(id),
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

    [EndpointSummary("Delete Mood")]
    [EndpointDescription("Deletes a Mood, but also removes the link from the cover image asset and all tracks.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteMood(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<DeleteMoodCommandResult> result = await Mediator.Send(
            new DeleteMoodCommand(id),
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

    [EndpointSummary("Rename Mood")]
    [EndpointDescription("Renames a Mood.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPatch("{id:guid}/name")]
    public async Task<ActionResult> RenameMood(
        [FromRoute] Guid id,
        [FromBody] RenameMoodRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<RenameMoodCommandResult> result = await Mediator.Send(
            new RenameMoodCommand(
                id,
                request.Name),
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
