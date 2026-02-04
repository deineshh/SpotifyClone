using MassTransit;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;

namespace SpotifyClone.Catalog.Infrastructure.Persistence.Services;

internal sealed class MassTransitIntegrationEventPublisher(
    IPublishEndpoint publishEndpoint)
    : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public Task PublishAsync(IntegrationEvent integrationEvent)
        => _publishEndpoint.Publish(integrationEvent);
}
