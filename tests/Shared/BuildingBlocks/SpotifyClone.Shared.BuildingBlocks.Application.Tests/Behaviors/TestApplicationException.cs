using SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Tests.Behaviors;

public sealed class TestApplicationException
        : ApplicationExceptionBase
{
    public TestApplicationException()
        : base("test") { }
}
