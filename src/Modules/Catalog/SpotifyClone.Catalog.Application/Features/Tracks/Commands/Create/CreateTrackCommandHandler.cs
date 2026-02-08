using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Albums.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Genres.ValueObjects;
using SpotifyClone.Catalog.Domain.Aggregates.Tracks;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Commands.Create;

internal sealed class CreateTrackCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CreateTrackCommand, CreateTrackCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CreateTrackCommandResult>> Handle(
        CreateTrackCommand request,
        CancellationToken cancellationToken)
    {
        var trackId = Guid.NewGuid();

        var track = Track.Create(
            TrackId.From(trackId),
            request.Title,
            request.ContainsExplicitContent,
            request.TrackNumber,
            AlbumId.From(request.AlbumId),
            request.MainArtists.Select(a => ArtistId.From(a)),
            request.FeaturedArtists.Select(a => ArtistId.From(a)),
            request.Genres.Select(g => GenreId.From(g)));

        await _unit.Tracks.AddAsync(track, cancellationToken);

        return new CreateTrackCommandResult(trackId);
    }
}
