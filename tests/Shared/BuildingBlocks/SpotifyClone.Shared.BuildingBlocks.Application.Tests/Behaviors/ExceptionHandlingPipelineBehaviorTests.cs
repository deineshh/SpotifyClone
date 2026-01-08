using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.BuildingBlocks.Application.Tests.Behaviors.TestExceptions;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Tests.Behaviors;

public sealed class ExceptionHandlingPipelineBehaviorTests
{
    private readonly Mock<ILogger<ExceptionHandlingPipelineBehavior<TestCommand, Result>>> _loggerMock = new();
    private readonly Mock<IDomainExceptionMapper> _mapperMock = new();
    private readonly ExceptionHandlingPipelineBehavior<TestCommand, Result> _behavior;

    public ExceptionHandlingPipelineBehaviorTests()
        => _behavior = new ExceptionHandlingPipelineBehavior<TestCommand, Result>(
            _mapperMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NoExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        RequestHandlerDelegate<Result> next = _ => Task.FromResult(Result.Success());

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_WarningLog_When_KnownDomainExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        var testDomainEx = new TestDomainException("Test domain exception");
        RequestHandlerDelegate<Result> next = _ => throw testDomainEx;

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(
                    "Domain exception occured while handling TestCommand: Test domain exception")),
                testDomainEx,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ErrorLog_When_UnknownDomainExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        var testDomainEx = new TestDomainException("Test domain exception");
        RequestHandlerDelegate<Result> next = _ => throw testDomainEx;
        _mapperMock
            .Setup(mapper => mapper.MapToError(It.IsAny<DomainExceptionBase>()))
            .Returns(CommonErrors.Unknown);

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(
                    "Unhandled domain exception occured while handling TestCommand.")),
                testDomainEx,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ErrorLog_When_NotDomainExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        var testEx = new TestException("Test exception");
        RequestHandlerDelegate<Result> next = _ => throw testEx;

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(
                    "Internal exception occured while handling TestCommand.")),
                testEx,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResultWithInternalError_When_NotDomainExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        RequestHandlerDelegate<Result> next = _ => throw new TestException("Test exception");

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e == CommonErrors.Internal);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResultWithNotInternalError_When_DomainExceptionIsThrown()
    {
        // Arrange
        var request = new TestCommand();
        RequestHandlerDelegate<Result> next = _ => throw new TestDomainException("Test domain exception");

        // Act
        Result result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotContain(CommonErrors.Internal);
    }
}
