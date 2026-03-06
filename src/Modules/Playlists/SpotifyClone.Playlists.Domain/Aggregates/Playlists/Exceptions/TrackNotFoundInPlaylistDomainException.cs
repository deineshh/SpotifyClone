using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Playlists.Domain.Aggregates.Playlists.Exceptions;

public sealed class TrackNotFoundInPlaylistDomainException(string message)
    : DomainExceptionBase(message);
