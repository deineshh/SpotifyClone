using FluentValidation;

namespace SpotifyClone.Catalog.Application.Features.Albums.Commands.Create;

public sealed class CreateAlbumCommandValidator
    : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumCommandValidator()
    {
    }
}
