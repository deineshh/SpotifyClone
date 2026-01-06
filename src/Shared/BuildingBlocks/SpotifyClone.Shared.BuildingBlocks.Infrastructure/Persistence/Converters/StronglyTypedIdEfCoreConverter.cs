using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.Persistence.Converters;

public abstract class StronglyTypedIdEfCoreConverter<TId, TValue> : ValueConverter<TId, TValue>
    where TId : StronglyTypedId<TValue>
    where TValue : notnull
{
    protected StronglyTypedIdEfCoreConverter(
        Expression<Func<TId, TValue>> toProvider,
        Expression<Func<TValue, TId>> fromProvider)
        : base(toProvider, fromProvider)
    {
    }
}
