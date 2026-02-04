using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.LinkAudioToTrack;

public sealed record LinkAudioToTrackCommand(
    Guid AudioId,
    Guid TrackId)
    : ICommand<LinkAudioToTrackCommandResult>;
