using SpotifyClone.Catalog.Application.Features.Tracks.Queries;
using SpotifyClone.Shared.Kernel.IDs;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface ITrackReadService
{
    Task<TrackDetailsResponse?> GetDetailsAsync(
        TrackId id,
        CancellationToken cancellationToken = default);
}
