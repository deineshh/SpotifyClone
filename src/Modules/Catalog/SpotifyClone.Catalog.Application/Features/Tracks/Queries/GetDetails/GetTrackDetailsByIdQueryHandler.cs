using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetDetails;

internal sealed class GetTrackDetailsByIdQueryHandler(
    ITrackReadService trackReadService,
    ILogger<GetTrackDetailsByIdQueryHandler> logger)
    : IQueryHandler<GetTrackDetailsByIdQuery, TrackDetailsResponse>
{
    private readonly ITrackReadService _trackReadService = trackReadService;
    private readonly ILogger<GetTrackDetailsByIdQueryHandler> _logger = logger;

    public async Task<Result<TrackDetailsResponse>> Handle(
        GetTrackDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting Track info {TrackId}", request.TrackId);

        TrackDetailsResponse? track = await _trackReadService.GetDetailsAsync(
            TrackId.From(request.TrackId),
            cancellationToken);

        if (track is null)
        {
            _logger.LogWarning(
                "Track {TrackId} not found", request.TrackId);

            return Result.Failure<TrackDetailsResponse>(TrackErrors.NotFound);
        }

        return track;
    }
}
