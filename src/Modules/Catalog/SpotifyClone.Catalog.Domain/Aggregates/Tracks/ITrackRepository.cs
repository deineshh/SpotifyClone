using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks;

public interface ITrackRepository
{
    Task AddAsync(
        Track track,
        CancellationToken cancellationToken = default);

    Task<Track?> GetByIdAsync(
        TrackId id,
        CancellationToken cancellationToken = default);
}
