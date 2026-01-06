using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Tests.Behaviors;

public sealed class ExceptionHandlingPipelineBehaviorTests
{
    private readonly Mock<ILogger<ExceptionHandlingPipelineBehavior<TestCommand, Result>>> _loggerMock =
        new();

    private readonly ExceptionHandlingPipelineBehavior<TestCommand, Result> _behavior;

    public ExceptionHandlingPipelineBehaviorTests()
        => _behavior = new ExceptionHandlingPipelineBehavior<TestCommand, Result>(_loggerMock.Object);

    [Fact]
    public async Task Handle_Should_ReturnResultWhenNoExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();

        RequestHandlerDelegate<Result> next =
            _ => Task.FromResult(Result.Success());

        // Act
        Result result = await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_RethrowOperationCanceledApplicationException()
    {
        // Arrange
        var request = new TestCommand();

        RequestHandlerDelegate<Result> next =
            _ => throw new OperationCanceledApplicationException();

        // Act
        Func<Task> act = () => _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledApplicationException>();
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_ReturnConcurrencyConflictWhenConcurrencyExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();

        RequestHandlerDelegate<Result> next =
            _ => throw new ConcurrencyConflictApplicationException();

        // Act
        Result result = await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e =>
            e == CommonErrors.ConcurrencyConflict);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<ConcurrencyConflictApplicationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnInternalErrorWhenApplicationExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();

        RequestHandlerDelegate<Result> next =
            _ => throw new TestApplicationException();

        // Act
        Result result = await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e =>
            e == CommonErrors.Internal);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<ApplicationExceptionBase>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_WorkWithGenericResult()
    {
        // Arrange
        var logger = new Mock<ILogger<ExceptionHandlingPipelineBehavior<TestCommand, Result<int>>>>();

        var behavior = new ExceptionHandlingPipelineBehavior<TestCommand, Result<int>>(
            logger.Object);

        RequestHandlerDelegate<Result<int>> next =
            _ => throw new ConcurrencyConflictApplicationException();

        // Act
        Result<int> result = await behavior.Handle(
            new TestCommand(),
            next,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e =>
            e == CommonErrors.ConcurrencyConflict);
    }

    [Fact]
    public async Task Handle_Should_ThrowWhenResponseTypeIsUnsupported()
    {
        // Arrange
        var logger = new Mock<ILogger<ExceptionHandlingPipelineBehavior<TestCommand, string>>>();

        var behavior = new ExceptionHandlingPipelineBehavior<TestCommand, string>(
            logger.Object);

        RequestHandlerDelegate<string> next =
            _ => throw new TestApplicationException();

        // Act
        Func<Task> act = () => behavior.Handle(
            new TestCommand(),
            next,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
