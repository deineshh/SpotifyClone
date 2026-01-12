using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

public sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IDomainExceptionMapper> mappers,
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull, Result
{
    private readonly IEnumerable<IDomainExceptionMapper> _mappers = mappers;
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
        catch (Exception ex)
        {
            Error error;

            if (ex is DomainExceptionBase domainEx)
            {
                IDomainExceptionMapper? mapper =
                    _mappers.FirstOrDefault(m => m.CanMap(domainEx));

                error = mapper?.MapToError(domainEx) ?? CommonErrors.Unknown;

                if (error == CommonErrors.Unknown)
                {
                    _logger.LogError(
                        domainEx,
                        "Unhandled domain exception while handling {RequestName}.",
                        typeof(TRequest).Name);
                }
                else
                {
                    _logger.LogWarning(
                        domainEx,
                        "Domain exception while handling {RequestName}: {Error}",
                        typeof(TRequest).Name,
                        error.Description);
                }
            }
            else
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception while handling {RequestName}.",
                    typeof(TRequest).Name);

                error = CommonErrors.Internal;
            }

            return CreateFailureResult(error);
        }
    }

    private static TResponse CreateFailureResult(Error error)
    {
        Type responseType = typeof(TResponse);
        object? failureResult;

        MethodInfo[] methods = typeof(Result).GetMethods(BindingFlags.Public | BindingFlags.Static);

        MethodInfo[] failureMethods = methods
            .Where(m => m.Name == "Failure" &&
                        m.GetParameters() is { Length: 1 } p &&
                        p[0].ParameterType == typeof(Error[]))
            .ToArray();

        if (failureMethods.Length != 2)
        {
            throw new InvalidOperationException("Expected exactly two Failure method overloads on Result.");
        }

        MethodInfo? nonGenericMethod = failureMethods.FirstOrDefault(m => !m.IsGenericMethod);
        MethodInfo? genericMethodDef = failureMethods.FirstOrDefault(m => m.IsGenericMethod);

        if (nonGenericMethod == null || genericMethodDef == null)
        {
            throw new InvalidOperationException("Failure method overloads not found on Result.");
        }

        Error[] errorsArray = new[] { error };

        if (responseType == typeof(Result))
        {
            failureResult = nonGenericMethod.Invoke(null, new object[] { errorsArray });
        }
        else if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = responseType.GetGenericArguments()[0];
            MethodInfo closedMethod = genericMethodDef.MakeGenericMethod(valueType);
            failureResult = closedMethod.Invoke(null, new object[] { errorsArray });
        }
        else
        {
            throw new InvalidOperationException($"Unexpected response type: {responseType}");
        }

        if (failureResult == null)
        {
            throw new InvalidOperationException("Failed to invoke Failure method.");
        }

        return (TResponse)failureResult;
    }
}
