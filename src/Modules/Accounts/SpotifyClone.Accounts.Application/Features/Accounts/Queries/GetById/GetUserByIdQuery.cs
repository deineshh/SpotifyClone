using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Queries;

namespace SpotifyClone.Accounts.Application.Features.Accounts.Queries.GetById;

public sealed record GetUserByIdQuery(
    Guid UserId)
    : IQuery<GetUserByIdQuery>;
