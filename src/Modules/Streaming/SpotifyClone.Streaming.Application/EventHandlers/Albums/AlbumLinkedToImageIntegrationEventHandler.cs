using MediatR;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Albums;
using SpotifyClone.Streaming.Application.Features.Media.Commands.AddLinkToImageAsset;

namespace SpotifyClone.Streaming.Application.EventHandlers.Albums;

internal sealed class AlbumLinkedToImageIntegrationEventHandler(
    IPublisher publisher)
    : INotificationHandler<AlbumLinkedToImageIntegrationEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task Handle(
        AlbumLinkedToImageIntegrationEvent notification,
        CancellationToken cancellationToken)
        => await _publisher.Publish(
            new AddLinkToImageAssetCommand(notification.ImageId),
            cancellationToken);
}
