using MassTransit;
using MediatR;
using SpotifyClone.Shared.IntegrationEvents.Catalog.Tracks;
using SpotifyClone.Streaming.Application.Features.Media.Commands.LinkAudioToTrack;

namespace SpotifyClone.Streaming.Infrastructure.Persistence.EventConsumers;

internal sealed class TrackCreatedConsumer(
    IMediator mediator)
    : IConsumer<TrackCreatedIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;

    public async Task Consume(ConsumeContext<TrackCreatedIntegrationEvent> context)
        => await _mediator.Send(new LinkAudioToTrackCommand(
            context.Message.AudioFileId,
            context.Message.TrackId));
}
