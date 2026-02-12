using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.PublishTrack;
using SpotifyClone.Api.Contracts.v1.Catalog.Tracks.UpdateInfo;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.Delete;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.PublishTrack;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnlinkFromAudioFile;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.UnpublishTrack;
using SpotifyClone.Catalog.Application.Features.Tracks.Commands.UpdateInfo;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetDetails;
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

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult> PublishTrack(
        [FromRoute] Guid id,
        [FromBody] PublishTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<PublishTrackCommandResult> result = await Mediator.Send(
            new PublishTrackCommand(
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

        return Ok();
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<ActionResult> UnpublishTrack(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnpublishTrackCommandResult> result = await Mediator.Send(
            new UnpublishTrackCommand(id),
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

    [HttpPost("{id:guid}/unlink-audio-file")]
    public async Task<ActionResult> UnlinkFromAudioFile(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnlinkTrackFromAudioFileCommandResult> result = await Mediator.Send(
            new UnlinkTrackFromAudioFileCommand(id),
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
            new GetTrackDetailsQuery(id),
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

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTrack(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<DeleteTrackCommandResult> result = await Mediator.Send(
            new DeleteTrackCommand(id),
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

    [HttpPut("{id:guid}/info")]
    public async Task<ActionResult> UpdateTrackInfo(
        [FromRoute] Guid id,
        [FromBody] UpdateTrackInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<UpdateTrackInfoCommandResult> result = await Mediator.Send(
            new UpdateTrackInfoCommand(
                id,
                request.Title,
                request.ReleaseDate,
                request.ContainsExplicitContent,
                request.TrackNumber),
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
