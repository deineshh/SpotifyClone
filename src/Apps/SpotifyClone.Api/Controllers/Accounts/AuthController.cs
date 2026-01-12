using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginUser;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.RegisterUser;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Accounts;

[Route("api/auth")]
public sealed class AuthController(IMediator mediator, IHostEnvironment hostEnvironment)
    : ApiController(mediator)
{
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterUserResponse>> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<Guid> result = await Mediator.Send(
            new RegisterUserCommand(
                request.Email,
                request.Password),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Created(
            uri: new Uri($"/api/auth/register/{result.Value}"),
            value: new RegisterUserResponse(
                result.Value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        Result<LoginUserResult> result = await Mediator.Send(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Errors);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth/refresh",
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);

        return Ok(
            new LoginUserResponse(
                result.Value.UserId,
                result.Value.AccessToken,
                result.Value.AccessTokenExpiresAt,
                result.Value.RefreshToken));
    }
}
