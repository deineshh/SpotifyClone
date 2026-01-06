using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Exceptions;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

public sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (OperationCanceledApplicationException)
        {
            throw;
        }
        catch (ConcurrencyConflictApplicationException ex)
        {
            _logger.LogWarning(
                ex,
                "Concurrency conflict while handling {RequestName}",
                typeof(TRequest).Name);

            return CreateFailure(CommonErrors.ConcurrencyConflict);
        }
        catch (EmailSendFailedApplicationException ex)
        {
            _logger.LogError(
                ex,
                "Email send failure while handling {RequestName}",
                typeof(TRequest).Name);

            return CreateFailure(CommonErrors.EmailSendFailed);
        }
        catch (ApplicationExceptionBase ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception while handling {RequestName}",
                typeof(TRequest).Name);

            return CreateFailure(CommonErrors.Internal);
        }
    }

    private static TResponse CreateFailure(Error error)
    {
        Type responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = responseType.GetGenericArguments()[0];

            MethodInfo failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Failure" && m.IsGenericMethod)
                .MakeGenericMethod(valueType);

            return (TResponse)failureMethod.Invoke(null, new object[] { new[] { error } })!;
        }

        throw new InvalidOperationException($"Unsupported response type {responseType.Name}.");
    }
}
