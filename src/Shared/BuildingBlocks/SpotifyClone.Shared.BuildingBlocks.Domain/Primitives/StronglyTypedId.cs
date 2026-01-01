namespace SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

public abstract record StronglyTypedId<TId>(TId Value) : ValueObject;
