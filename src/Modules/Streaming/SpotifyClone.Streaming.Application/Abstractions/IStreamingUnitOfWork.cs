using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;

namespace SpotifyClone.Streaming.Application.Abstractions;

public interface IStreamingUnitOfWork : IUnitOfWork
{
    IAudioAssetRepository AudioAssets { get; }
}
