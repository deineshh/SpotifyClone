using FluentValidation;

namespace SpotifyClone.Streaming.Application.Features.Media.Commands.MarkAsOrphaned;

public sealed class MarkAsOrphanedCommandValidator
    : AbstractValidator<MarkAsOrphanedCommand>
{
    public MarkAsOrphanedCommandValidator()
        => RuleFor(x => x.AudioAssetId)
        .NotNull().WithMessage("Audio Asset ID is required.")
        .NotEqual(Guid.Empty).WithMessage("Audio Asset ID is required.");
}
