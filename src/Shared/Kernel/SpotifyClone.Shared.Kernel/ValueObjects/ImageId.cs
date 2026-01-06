using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.Kernel.ValueObjects;

public sealed record ImageId(Guid Value) : StronglyTypedId<Guid>(Value);
