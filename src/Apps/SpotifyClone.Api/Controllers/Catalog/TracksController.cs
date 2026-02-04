using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.Create;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Catalog;

[Route("api/v1/tracks")]
public sealed class MediaController(IMediator mediator)
    : ApiController(mediator)
{
    [HttpPost]
    public async Task<ActionResult<CreateTrackResponse>> CreateTrack(
        [FromBody] CreateTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CreateTrackCommandResult> createResult = await Mediator.Send(
            new CreateTrackCommand(
                request.Title,
                request.ContainsExplicitContent,
                request.TrackNumber,
                request.AlbumId,
                request.MainArtists,
                request.FeaturedArtists,
                request.Genres,
                request.AudioFileId),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateTrackCommandResult createResultData = createResult.Value;

        return Accepted(new CreateTrackResponse(
            createResultData.TrackId));
    }
}
