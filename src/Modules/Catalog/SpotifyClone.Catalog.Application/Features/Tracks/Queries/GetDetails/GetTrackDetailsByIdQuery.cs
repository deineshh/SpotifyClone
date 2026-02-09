using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;

namespace SpotifyClone.Catalog.Application.Features.Tracks.Queries.GetDetails;

public sealed record GetTrackDetailsByIdQuery(
    Guid TrackId)
    : IQuery<TrackDetailsResponse>;
