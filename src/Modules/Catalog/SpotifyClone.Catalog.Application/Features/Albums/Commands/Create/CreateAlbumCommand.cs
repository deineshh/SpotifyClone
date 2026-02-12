using SpotifyClone.Catalog.Application.Abstractions;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;

public sealed record CreateAlbumCommand(
    )
    : ICatalogPersistentCommand<CreateAlbumCommandResult>;
