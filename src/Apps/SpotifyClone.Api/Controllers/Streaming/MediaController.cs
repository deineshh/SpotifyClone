using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Streaming.Media.UploadAudioAsset;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;

namespace SpotifyClone.Api.Controllers.Streaming;

[Route("api/v1/media")]
public sealed class MediaController(IMediator mediator)
    : ApiController(mediator)
{
    [HttpPost("audio")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadAudioAssetResponse>> UploadAudioAsset(
        [FromForm] UploadAudioAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        using Stream stream = request.File.OpenReadStream();

        Result<UploadAudioAssetCommandResult> result = await Mediator.Send(
            new UploadAudioAssetCommand(
                request.Title,
                request.Artist,
                request.File.FileName,
                stream),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        UploadAudioAssetCommandResult resultData = result.Value;

        return new UploadAudioAssetResponse(
            resultData.MediaId);
    }
}
