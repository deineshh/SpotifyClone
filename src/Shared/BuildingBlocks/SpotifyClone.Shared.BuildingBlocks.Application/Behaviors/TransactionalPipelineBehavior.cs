using MediatR;
using Microsoft.Extensions.Logging;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Commands;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Shared.BuildingBlocks.Application.Behaviors;

public sealed class TransactionalPipelineBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<TransactionalPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPersistentCommand
    where TResponse : Result
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<TransactionalPipelineBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Beginning transaction for {RequestType}", typeof(TRequest).Name);

        TResponse response = await next(cancellationToken);

        if (response.IsFailure)
        {
            return response;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Committed transaction for {RequestType}", typeof(TRequest).Name);

        return response;
    }
}
