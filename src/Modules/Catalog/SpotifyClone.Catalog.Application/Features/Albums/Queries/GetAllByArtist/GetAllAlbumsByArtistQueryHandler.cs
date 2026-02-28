using SpotifyClone.Catalog.Application.Abstractions.Data;
using SpotifyClone.Catalog.Application.Errors;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Albums.Queries.GetAllByArtist;

internal sealed class GetAllAlbumsByArtistQueryHandler(
    IArtistReadService artistReadService,
    IAlbumReadService albumReadService)
    : IQueryHandler<GetAllAlbumsByArtistQuery, AlbumList>
{
    private readonly IArtistReadService _artistReadService = artistReadService;
    private readonly IAlbumReadService _albumReadService = albumReadService;

    public async Task<Result<AlbumList>> Handle(
        GetAllAlbumsByArtistQuery request,
        CancellationToken cancellationToken)
    {
        var artistId = ArtistId.From(request.ArtistId);

        bool artistExists = await _artistReadService.ExistsAsync(
            artistId, cancellationToken);
        if (!artistExists)
        {
            return Result.Failure<AlbumList>(ArtistErrors.NotFound);
        }

        IEnumerable<AlbumSummary> albums = await _albumReadService.GetAllByArtistIdAsync(
            artistId, cancellationToken);

        return new AlbumList(albums.ToList().AsReadOnly());
    }
}
