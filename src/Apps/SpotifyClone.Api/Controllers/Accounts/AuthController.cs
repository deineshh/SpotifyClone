using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Logout;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithPassword;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithRefreshToken;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.RegisterUser;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Accounts;

[Route("api/v1/auth")]
public sealed class AuthController(IMediator mediator, IHostEnvironment hostEnvironment)
    : ApiController(mediator)
{
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    private readonly CookieOptions _cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = !hostEnvironment.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Path = "/",
        MaxAge = TimeSpan.FromDays(30)
    };

[HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterUserResponse>> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<RegisterUserCommandResult> registrationResult = await Mediator.Send(
            new RegisterUserCommand(
                request.Email,
                request.Password,
                request.DisplayName,
                request.BirthDate,
                request.Gender),
            cancellationToken);
        if (registrationResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                registrationResult,
                HttpContext);

            int? statusCode = problemDetails.Status;

            if (registrationResult.Errors.Contains(AuthErrors.EmailAlreadyInUse))
            {
                statusCode = (int)HttpStatusCode.Conflict;
                problemDetails.Status = statusCode;
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
        }

        Result<LoginWithPasswordCommandResult> loginResult = await Mediator.Send(
            new LoginWithPasswordCommand(
                request.Email,
                request.Password),
            cancellationToken);
        if (loginResult.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                loginResult,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        RegisterUserCommandResult registrationResultData = registrationResult.Value;
        LoginWithPasswordCommandResult loginResultData = loginResult.Value;

        Response.Cookies.Append("refreshToken", loginResultData.RefreshToken, _cookieOptions);

        return Created(
            uri: new Uri($"/api/auth/register/{registrationResult.Value}"),
            value: new RegisterUserResponse(
                registrationResultData.UserId,
                registrationResultData.Email,
                registrationResultData.DisplayName,
                registrationResultData.BirthDate,
                registrationResultData.Gender,
                loginResultData.AccessToken));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginWithPasswordResponse>> Login(
        LoginWithPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result<LoginWithPasswordCommandResult> result = await Mediator.Send(
            new LoginWithPasswordCommand(request.Email, request.Password),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        LoginWithPasswordCommandResult resultData = result.Value;

        Response.Cookies.Append("refreshToken", resultData.RefreshToken, _cookieOptions);

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
        CancellationToken cancellationToken)
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token not found.");
        }

        Result<LoginWithRefreshTokenCommandResult> result = await Mediator.Send(
            new LoginWithRefreshTokenCommand(refreshToken),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        LoginWithRefreshTokenCommandResult resultData = result.Value;

        Response.Cookies.Append("refreshToken", resultData.RefreshToken, _cookieOptions);

        return Ok(
            new LoginWithRefreshTokenResponse(
                result.Value.AccessToken));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        Result result = await Mediator.Send(new LogoutCommand(), cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        Response.Cookies.Delete("refreshToken", _cookieOptions);

        return Ok();
    }
}
