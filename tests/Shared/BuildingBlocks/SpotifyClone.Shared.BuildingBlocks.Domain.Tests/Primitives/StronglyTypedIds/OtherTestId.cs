using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Domain.Tests.Primitives.StronglyTypedIds;

internal sealed record OtherTestId(Guid Value) : StronglyTypedId<Guid>(Value);
