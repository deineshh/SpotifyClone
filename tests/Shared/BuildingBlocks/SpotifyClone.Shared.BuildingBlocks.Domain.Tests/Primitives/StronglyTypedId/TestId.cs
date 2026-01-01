using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.StronglyTypedId;

internal sealed record TestId(Guid Value) : StronglyTypedId<Guid>(Value);
