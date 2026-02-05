using SpotifyClone.Streaming.Application.Abstractions;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.LinkAudioToTrack;

public sealed record LinkAudioToTrackCommand(
    Guid AudioId,
    Guid TrackId)
    : IStreamingPersistentCommand<LinkAudioToTrackCommandResult>;
