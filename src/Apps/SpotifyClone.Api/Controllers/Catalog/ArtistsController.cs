using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.Create;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Artists.Queries;
using SpotifyClone.Catalog.Application.Features.Artists.Queries.GetDetails;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Catalog;

[Tags("Catalog Module")]
[Route("api/v1/artists")]
public sealed class ArtistsController(IMediator mediator)
    : ApiController(mediator)
{
    [EndpointSummary("Create Artist")]
    [EndpointDescription("Creates an Artist in a non-verified state. " +
        "Note: Non-verified artists cannot have custom bio, banner or gallery.")]
    [ProducesResponseType(typeof(CreateArtistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<CreateArtistResponse>> CreateAlbum(
        [FromBody] CreateArtistRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<CreateArtistCommandResult> createResult = await Mediator.Send(
            new CreateArtistCommand(request.Name),
            cancellationToken);
        if (createResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                createResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        CreateArtistCommandResult createResultData = createResult.Value;

        return CreatedAtAction(nameof(GetArtistDetails),
            new { id = createResultData.ArtistId },
            new CreateArtistResponse(
                createResultData.ArtistId));
    }

    [EndpointSummary("Get Artist Details")]
    [EndpointDescription("Returns all the necessary Artist details.")]
    [ProducesResponseType(typeof(ArtistDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArtistDetailsResponse>> GetArtistDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<ArtistDetailsResponse> result = await Mediator.Send(
            new GetArtistDetailsQuery(id),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok(result.Value);
    }
}
