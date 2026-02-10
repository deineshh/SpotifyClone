using SpotifyClone.Streaming.Application.Abstractions;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.MarkAsOrphaned;

public sealed record MarkAsOrphanedCommand(
    Guid AudioAssetId)
    : IStreamingPersistentCommand<MarkAsOrphanedCommandResult>;
