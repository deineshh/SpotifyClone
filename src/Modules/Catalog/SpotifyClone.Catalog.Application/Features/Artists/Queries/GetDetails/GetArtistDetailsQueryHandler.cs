using Microsoft.Extensions.Logging;
using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Artists.Queries.GetDetails;

internal sealed class GetArtistDetailsQueryHandler(
    IArtistReadService artistReadService,
    ILogger<GetArtistDetailsQueryHandler> logger)
    : IQueryHandler<GetArtistDetailsQuery, ArtistDetails>
{
    private readonly IArtistReadService _artistReadService = artistReadService;
    private readonly ILogger<GetArtistDetailsQueryHandler> _logger = logger;

    public async Task<Result<ArtistDetails>> Handle(
        GetArtistDetailsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting Artist info {ArtistId}", request.ArtistId);

        ArtistDetails? artist = await _artistReadService.GetDetailsAsync(
            ArtistId.From(request.ArtistId),
            cancellationToken);

        if (artist is null)
        {
            _logger.LogWarning(
                "Artist {ArtistId} not found", request.ArtistId);

            return Result.Failure<ArtistDetails>(ArtistErrors.NotFound);
        }

        return artist;
    }
}
