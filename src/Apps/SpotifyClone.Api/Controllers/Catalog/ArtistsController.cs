using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.AddGalleryImage;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.Create;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.EditProfile;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.LinkNewAvatar;
using SpotifyClone.Api.Contracts.v1.Catalog.Artists.LinkNewBanner;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Catalog.Application.Features.Albums.Queries;
using SpotifyClone.Catalog.Application.Features.Albums.Queries.GetAllByArtist;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.AddGalleryImage;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Ban;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Create;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.EditProfile;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.LinkNewAvatar;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.LinkNewBanner;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.RemoveGalleryImageFromArtist;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Unban;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.UnlinkAvatar;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.UnlinkBanner;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Unverify;
using SpotifyClone.Catalog.Application.Features.Artists.Commands.Verify;
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
    [ProducesResponseType(typeof(ArtistDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ArtistDetails>> GetArtistDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<ArtistDetails> result = await Mediator.Send(
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

    [EndpointSummary("Get all Artist's albums")]
    [EndpointDescription("Returns all Albums owned by a certain Artist.")]
    [ProducesResponseType(typeof(ArtistDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:guid}/albums")]
    public async Task<ActionResult<AlbumList>> GetAlbumsByArtistDetails(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<AlbumList> result = await Mediator.Send(
            new GetAllAlbumsByArtistQuery(id),
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

    [EndpointSummary("Link Artist to new avatar image")]
    [EndpointDescription("Links an Artist to a new avatar image.")]
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

    [EndpointSummary("Unlink Artist from avatar image")]
    [EndpointDescription("Unlinks an Artist from it's avatar image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/avatar")]
    public async Task<ActionResult> UnlinkAvatarImage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnlinkAvatarFromArtistCommandResult> result = await Mediator.Send(
            new UnlinkAvatarFromArtistCommand(id),
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

    [EndpointSummary("Link Artist to new banner image")]
    [EndpointDescription("Links an Artist to a new banner image.")]
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

    [EndpointSummary("Unlink Artist from banner image")]
    [EndpointDescription("Unlinks an Artist from it's banner image.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/banner")]
    public async Task<ActionResult> UnlinkBannerImage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnlinkBannerFromArtistCommandResult> result = await Mediator.Send(
            new UnlinkBannerFromArtistCommand(id),
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

    [EndpointSummary("Verify Artist")]
    [EndpointDescription("Verifies an artist to unlock new features (bio, banner, gallery etc).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/verify")]
    public async Task<ActionResult> VerifyArtist(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<VerifyArtistCommandResult> result = await Mediator.Send(
            new VerifyArtistCommand(id),
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

    [EndpointSummary("Unverify Artist")]
    [EndpointDescription("Unverifies a verified artist to prevent from additional features.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/verify")]
    public async Task<ActionResult> UnverifyArtist(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Result<UnverifyArtistCommandResult> result = await Mediator.Send(
            new UnverifyArtistCommand(id),
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

    [EndpointSummary("Edit Artist profile")]
    [EndpointDescription("Edits an Artist's profile info.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/profile")]
    public async Task<ActionResult> EditArtistProfile(
        [FromRoute] Guid id,
        [FromBody] EditArtistProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<EditArtistProfileCommandResult> result = await Mediator.Send(
            new EditArtistProfileCommand(
                id,
                request.Name,
                request.Bio),
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

    [EndpointSummary("Add gallery image to Artist")]
    [EndpointDescription("Adds a gallery image to a verified artist.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:guid}/gallery")]
    public async Task<ActionResult> AddGalleryImageToArtist(
        [FromRoute] Guid id,
        [FromBody] AddGalleryImageToArtistRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<AddGalleryImageToArtistCommandResult> result = await Mediator.Send(
            new AddGalleryImageToArtistCommand(
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

    [EndpointSummary("Remove gallery image from Artist")]
    [EndpointDescription("Removes a gallery image from a verified artist.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:guid}/gallery/{imageId:guid}")]
    public async Task<ActionResult> AddGalleryImageToArtist(
        [FromRoute] Guid id,
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken = default)
    {
        Result<RemoveGalleryImageFromArtistCommandResult> result = await Mediator.Send(
            new RemoveGalleryImageFromArtistCommand(id, imageId),
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
