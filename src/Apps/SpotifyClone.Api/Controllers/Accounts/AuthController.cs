using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Logout;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithPassword;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithRefreshToken;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginWithPasswordResponse>> Login(
        LoginWithPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result<LoginWithPasswordResult> result = await Mediator.Send(
            new LoginWithPasswordCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Errors);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_hostEnvironment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth/refresh",
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);

        return Ok(
            new LoginWithPasswordResponse(
                result.Value.AccessToken));
    }

    [Authorize]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginWithRefreshTokenResponse>> Refresh(
        LoginWithRefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        Result<LoginWithRefreshTokenResult> result = await Mediator.Send(
            new LoginWithRefreshTokenCommand(request.RefreshToken),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_hostEnvironment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth/refresh",
            MaxAge = TimeSpan.FromDays(30)
        };

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, cookieOptions);

        return Ok(
            new LoginWithRefreshTokenResponse(
                result.Value.AccessToken));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Result result = await Mediator.Send(new LogoutCommand(), cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok();
    }
}
