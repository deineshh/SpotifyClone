using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Errors;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;
using SpotifyClone.Shared.BuildingBlocks.Domain.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

public sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    IDomainExceptionMapper mapper,
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull, Result
{
    private readonly IDomainExceptionMapper _mapper = mapper;
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
            if (ex is not DomainExceptionBase domainEx)
            {
                _logger.LogError(
                ex,
                "Internal exception occured while handling {RequestName}.",
                typeof(TRequest).Name);

                return (TResponse)Result.Failure(CommonErrors.Internal);
            }

            Error error = _mapper.MapToError(domainEx);

            if (error == CommonErrors.Unknown)
            {
                _logger.LogError(
                 domainEx,
                 "Unhandled domain exception occured while handling {RequestName}.",
                 typeof(TRequest).Name);
            }
            else
            {
                _logger.LogWarning(
                    domainEx,
                    "Domain exception occured while handling {RequestName}: {ErrorDescription}",
                    typeof(TRequest).Name,
                    domainEx.Message);
            }

            return (TResponse)Result.Failure(_mapper.MapToError(domainEx));
        }
    }
}
