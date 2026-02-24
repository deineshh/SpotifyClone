using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.LinkNewAvatar;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.LinkNewBanner;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Ban;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.LinkNewAvatar;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.LinkNewBanner;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Unban;
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

    [EndpointSummary("Link Artist to new Avatar image")]
    [EndpointDescription("Links an Artist to a new Avatar image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/avatar")]
    public async Task<ActionResult> LinkNewAvatarImage(
        [FromRoute] Guid id,
        [FromBody] LinkNewAvatarToArtistRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewAvatarToArtistCommandResult> result = await Mediator.Send(
            new LinkNewAvatarToArtistCommand(
                id,
                request.ImageId,
                request.ImageWidth,
                request.ImageHeight,
                request.ImageFileType,
                request.ImageSizeInBytes),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return NoContent();
    }

    [EndpointSummary("Link Artist to new Banner image")]
    [EndpointDescription("Links an Artist to a new Banner image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/banner")]
    public async Task<ActionResult> LinkNewBannerImage(
        [FromRoute] Guid id,
        [FromBody] LinkNewBannerToArtistRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LinkNewBannerToArtistCommandResult> result = await Mediator.Send(
            new LinkNewBannerToArtistCommand(
                id,
                request.ImageId,
                request.ImageWidth,
                request.ImageHeight,
                request.ImageFileType,
                request.ImageSizeInBytes),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return NoContent();
    }

    [EndpointSummary("Ban Artist")]
    [EndpointDescription("Bans an artist and unpublishes all it's releases (albums & tracks).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> BanArtist(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<BanArtistCommandResult> result = await Mediator.Send(
            new BanArtistCommand(id),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return NoContent();
    }

    [EndpointSummary("Unban Artist")]
    [EndpointDescription("Unbans an artist but not publishes back it's releases (albums & tracks).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/unban")]
    public async Task<ActionResult> UnbanArtist(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnbanArtistCommandResult> result = await Mediator.Send(
            new UnbanArtistCommand(id),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return NoContent();
    }
}
