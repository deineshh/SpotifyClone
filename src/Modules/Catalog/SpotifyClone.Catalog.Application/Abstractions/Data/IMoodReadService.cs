using SpotifyClone.Catalog.Application.Features.Moods.Queries;
using SpotifyClone.Catalog.Domain.Aggregates.Moods.ValueObjects;

namespace SpotifyClone.Catalog.Application.Abstractions.Data;

public interface IMoodReadService
{
    Task<MoodDetailsResponse?> GetDetailsAsync(
        MoodId id,
        CancellationToken cancellationToken = default);
}
