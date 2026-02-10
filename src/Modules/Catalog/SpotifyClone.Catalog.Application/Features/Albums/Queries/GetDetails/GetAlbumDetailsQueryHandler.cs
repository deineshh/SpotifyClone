using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Queries.GetDetails;

internal sealed class GetAlbumDetailsQueryHandler(
    IAlbumReadService albumReadService,
    ILogger<GetAlbumDetailsQueryHandler> logger)
    : IQueryHandler<GetAlbumDetailsQuery, AlbumDetailsResponse>
{
    private readonly IAlbumReadService _albumReadService = albumReadService;
    private readonly ILogger<GetAlbumDetailsQueryHandler> _logger = logger;

    public async Task<Result<AlbumDetailsResponse>> Handle(
        GetAlbumDetailsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting Album info {AlbumId}", request.AlbumId);

        AlbumDetailsResponse? album = await _albumReadService.GetDetailsAsync(
            AlbumId.From(request.AlbumId),
            cancellationToken);

        if (album is null)
        {
            _logger.LogWarning(
                "Album {AlbumId} not found", request.AlbumId);

            return Result.Failure<AlbumDetailsResponse>(AlbumErrors.NotFound);
        }

        return album;
    }
}
