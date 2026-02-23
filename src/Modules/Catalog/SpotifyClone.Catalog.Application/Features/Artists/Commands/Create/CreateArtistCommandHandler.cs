using SpotifyClone.Catalog.Application.Abstractions;
using SpotifyClone.Catalog.Domain.Aggregates.Artists;
using SpotifyClone.Catalog.Domain.Aggregates.Artists.ValueObjects;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Catalog.Application.Features.Artists.Commands.Create;

internal sealed class CreateArtistCommandHandler(
    ICatalogUnitOfWork unit)
    : ICommandHandler<CreateArtistCommand, CreateArtistCommandResult>
{
    private readonly ICatalogUnitOfWork _unit = unit;

    public async Task<Result<CreateArtistCommandResult>> Handle(
        CreateArtistCommand request,
        CancellationToken cancellationToken)
    {
        var artist = Artist.Create(
            ArtistId.From(Guid.NewGuid()),
            request.Name);

        await _unit.Artists.AddAsync(artist, cancellationToken);

        return new CreateArtistCommandResult(artist.Id.Value);
    }
}
