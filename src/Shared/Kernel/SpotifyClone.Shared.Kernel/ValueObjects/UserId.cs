using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.Kernel.ValueObjects;

public sealed record UserId(Guid Value) : StronglyTypedId<Guid>(Value);
