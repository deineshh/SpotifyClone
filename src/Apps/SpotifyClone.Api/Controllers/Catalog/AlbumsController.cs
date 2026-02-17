using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.PublishAlbum;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.Delete;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.PublishAlbum;
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
}
