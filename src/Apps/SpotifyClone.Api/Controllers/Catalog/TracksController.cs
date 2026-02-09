using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.PublishTrack;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.UnpublishTrack;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.PublishTrack;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnpublishTrack;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetDetails;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using Twilio.Http;

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
                request.Moods),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateTrackCommandResult createResultData = createResult.Value;

        return Ok(new CreateTrackResponse(
            createResultData.TrackId));
    }

    [HttpPost("publish")]
    public async Task<ActionResult> PublishTrack(
        [FromBody] PublishTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<PublishTrackCommandResult> result = await Mediator.Send(
            new PublishTrackCommand(
                request.TrackId,
                request.ReleaseDate),
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

    [HttpPost("unpublish")]
    public async Task<ActionResult> UnpublishTrack(
        [FromBody] UnpublishTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<UnpublishTrackCommandResult> result = await Mediator.Send(
            new UnpublishTrackCommand(
                request.TrackId),
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrackDetailsResponse>> GetTrackDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<TrackDetailsResponse> result = await Mediator.Send(
            new GetTrackDetailsByIdQuery(id),
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
}
