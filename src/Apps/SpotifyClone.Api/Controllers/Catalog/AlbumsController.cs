using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Albums.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.Create;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;
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

        //return Created(new Uri(
        //    $"api/v1/albums/{createResultData.AlbumId}"),
        //    new CreateTrackResponse(
        //        createResultData.AlbumId));

        return Ok(new CreateAlbumResponse(
                createResultData.AlbumId));
    }
}
