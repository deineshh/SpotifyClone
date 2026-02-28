using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Genres.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Genres.LinkNewCover;
using SpotifyClone.Api.Contracts.v1.Catalog.Genres.Rename;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Genres.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Genres.Commands.Delete;
using SpotifyClone.Catalog.Application.Features.Genres.Commands.LinkNewCover;
using SpotifyClone.Catalog.Application.Features.Genres.Commands.Rename;
using SpotifyClone.Catalog.Application.Features.Genres.Commands.UnlinkCover;
using SpotifyClone.Catalog.Application.Features.Genres.Queries;
using SpotifyClone.Catalog.Application.Features.Genres.Queries.GetDetails;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetAllByGenre;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Catalog;

[Tags("Catalog Module")]
[Route("api/v1/genres")]
public sealed class GenresController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Create Genre")]
    [EndpointDescription("Creates a Genre without cover.")]
    [ProducesResponseType(typeof(CreateGenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<CreateGenreResponse>> CreateGenre(
        [FromBody] CreateGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CreateGenreCommandResult> createResult = await Mediator.Send(
            new CreateGenreCommand(request.Name),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateGenreCommandResult createResultData = createResult.Value;

        return CreatedAtAction(nameof(GetGenreDetails),
            new { id = createResultData.GenreId },
            new CreateGenreResponse(
                createResultData.GenreId));
    }

    [EndpointSummary("Get Genre Details")]
    [EndpointDescription("Returns all the necessary Genre details.")]
    [ProducesResponseType(typeof(GenreDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GenreDetails>> GetGenreDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<GenreDetails> result = await Mediator.Send(
            new GetGenreDetailsQuery(id),
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

    [EndpointSummary("Get all Tracks by a Genre")]
    [EndpointDescription("Returns all Tracks by a specific Genre.")]
    [ProducesResponseType(typeof(TrackList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}/tracks")]
    public async Task<ActionResult<TrackList>> GetAllTracksByGenreDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<TrackList> result = await Mediator.Send(
            new GetAllTracksByGenreQuery(id),
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

    [EndpointSummary("Link Genre to new cover image")]
    [EndpointDescription("Links a Genre to a new cover image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/cover")]
    public async Task<ActionResult> LinkNewCoverImage(
        [FromRoute] Guid id,
        [FromBody] LinkNewCoverToGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewCoverToGenreCommandResult> result = await Mediator.Send(
            new LinkNewCoverToGenreCommand(
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

    [EndpointSummary("Unlink Genre from cover image")]
    [EndpointDescription("Unlinks a Genre from it's cover image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/cover")]
    public async Task<ActionResult> UnlinkCoverImage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnlinkCoverFromGenreCommandResult> result = await Mediator.Send(
            new UnlinkCoverFromGenreCommand(id),
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

    [EndpointSummary("Delete Genre")]
    [EndpointDescription("Deletes a Genre, but also removes the link from the cover image asset and all tracks.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteGenre(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<DeleteGenreCommandResult> result = await Mediator.Send(
            new DeleteGenreCommand(id),
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

    [EndpointSummary("Rename Genre")]
    [EndpointDescription("Renames a Genre.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPatch("{id:guid}/name")]
    public async Task<ActionResult> RenameGenre(
        [FromRoute] Guid id,
        [FromBody] RenameGenreRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<RenameGenreCommandResult> result = await Mediator.Send(
            new RenameGenreCommand(
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
