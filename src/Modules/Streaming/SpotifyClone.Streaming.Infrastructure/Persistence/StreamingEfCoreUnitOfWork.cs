using MediatR;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence;
using SpotifyClone.Streaming.Application.Abstractions;
using SpotifyClone.Streaming.Domain.Aggregates.AudioAssets;
using SpotifyClone.Streaming.Infrastructure.Persistence.Database;

namespace SpotifyClone.Streaming.Infrastructure.Persistence;

internal sealed class StreamingEfCoreUnitOfWork(
    StreamingAppDbContext context,
    IAudioAssetRepository audioAssets,
    IPublisher publisher)
    : EfCoreUnitOfWorkBase<StreamingAppDbContext>(context, publisher),
    IStreamingUnitOfWork
{
    public IAudioAssetRepository AudioAssets => audioAssets;
}
