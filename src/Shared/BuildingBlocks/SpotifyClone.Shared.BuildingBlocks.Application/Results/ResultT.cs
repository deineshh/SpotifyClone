using SpotifyClone.Shared.BuildingBlocks.Application.Errors;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Results;

public class Result<TValue> : Result
{
    public TValue Value { get; }

    protected internal Result(TValue value, bool isSuccess, params Error[] errors)
        : base(isSuccess, errors)
        => Value = value;

    public static implicit operator Result<TValue>(TValue value)
        => new Result<TValue>(value, true);
}
