using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Streaming.Media.GetAudioAsset;
using SpotifyClone.Api.Contracts.v1.Streaming.Media.GetImageAsset;
using SpotifyClone.Api.Contracts.v1.Streaming.Media.UploadAudioAsset;
using SpotifyClone.Api.Contracts.v1.Streaming.Media.UploadImageAsset;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Streaming.Application.Features.Media.Commands.UploadAudioAsset;
using SpotifyClone.Streaming.Application.Features.Media.Commands.UploadImageAsset;
using SpotifyClone.Streaming.Application.Features.Media.Queries.GetAudioAsset;
using SpotifyClone.Streaming.Application.Features.Media.Queries.GetImageAsset;

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
            resultData.AudioId);
    }

    //[Authorize]
    [HttpGet("audio")]
    public async Task<ActionResult<GetAudioAssetResponse>> GetAudioAsset(
        [FromQuery] GetAudioAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<GetAudioAssetQueryResult> result = await Mediator.Send(
            new GetAudioAssetQuery(
                request.AudioId),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);
            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        GetAudioAssetQueryResult resultData = result.Value;

        return new GetAudioAssetResponse(
            resultData.AudioId,
            resultData.HlsUrl,
            resultData.DashUrl);
    }

    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadImageAssetResponse>> UploadImageAsset(
        [FromForm] UploadImageAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        using Stream stream = request.File.OpenReadStream();

        Result<UploadImageAssetCommandResult> result = await Mediator.Send(
            new UploadImageAssetCommand(
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

        UploadImageAssetCommandResult resultData = result.Value;

        return new UploadImageAssetResponse(
            resultData.ImageId);
    }

    //[Authorize]
    [HttpGet("images")]
    public async Task<ActionResult<GetImageAssetResponse>> GetImageAsset(
        [FromQuery] GetImageAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<GetImageAssetQueryResult> result = await Mediator.Send(
            new GetImageAssetQuery(
                request.ImageId),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);
            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        GetImageAssetQueryResult resultData = result.Value;

        return new GetImageAssetResponse(
            resultData.ImageId,
            resultData.WebpUrl);
    }
}
