using MediatR;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Albums;
using SpotifyClone.Streaming.Application.Features.Media.Commands.RemoveLinkFromImageAsset;

namespace SpotifyClone.Streaming.Application.EventHandlers.Albums;

internal sealed class AlbumUnlinkedFromImageIntegrationEventHandler(
    IPublisher publisher)
    : INotificationHandler<AlbumUnlinkedFromImageIntegrationEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task Handle(
        AlbumUnlinkedFromImageIntegrationEvent notification,
        CancellationToken cancellationToken)
        => await _publisher.Publish(
            new RemoveLinkFromImageAssetCommand(notification.ImageId),
            cancellationToken);
}
