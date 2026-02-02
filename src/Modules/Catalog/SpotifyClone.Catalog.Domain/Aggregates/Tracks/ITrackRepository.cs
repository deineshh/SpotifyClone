using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Domain.Aggregates.Tracks;

public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(
        TrackId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Track track,
        CancellationToken cancellationToken = default);
}
