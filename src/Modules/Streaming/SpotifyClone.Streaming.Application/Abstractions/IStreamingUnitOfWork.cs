using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Domain.Aggregates.ImageAssets;

namespace SpotifyClone.Streaming.Application.Abstractions;

public interface IStreamingUnitOfWork : IUnitOfWork
{
    IAudioAssetRepository AudioAssets { get; }
    IImageAssetRepository ImageAssets { get; }
}
