using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Services;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IntegrationEvent integrationEvent);
}
