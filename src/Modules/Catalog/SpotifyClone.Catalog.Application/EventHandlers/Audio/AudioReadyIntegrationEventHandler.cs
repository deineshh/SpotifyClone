using MediatR;
using SpotifyClone.Catalog.Application.Jobs;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;
using SpotifyClone.Shared.IntegrationEvents.Streaming.AudioAssets;

namespace SpotifyClone.Catalog.Application.EventHandlers.Audio;

internal sealed class AudioReadyIntegrationEventHandler(
    IBackgroundJobService jobService)
    : INotificationHandler<AudioReadyIntegrationEvent>
{
    private readonly IBackgroundJobService _jobService = jobService;

    public async Task Handle(AudioReadyIntegrationEvent notification, CancellationToken cancellationToken)
        => _jobService.Enqueue<MarkTrackAsReadyToPublishJob>(job =>
            job.ProcessAsync(notification.TrackId));
}
