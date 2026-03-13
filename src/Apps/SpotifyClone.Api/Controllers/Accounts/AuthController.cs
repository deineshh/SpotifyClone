using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithPassword;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.LoginWithRefreshToken;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Logout;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Confirm;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Request;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Verify;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.SendVerificationSms;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.VerifyEmail;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.VerifyPhoneNumber;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithPassword;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.LoginWithRefreshToken;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.PasswordReset;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.RegisterUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.SendVerificationSms;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.VerifyEmail;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.VerifyPhoneNumber;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Auth;
using SpotifyClone.Shared.BuildingBlocks.Application.Results;

namespace SpotifyClone.Api.Controllers.Accounts;

[Route("api/v1/auth")]
public sealed class AuthController(IMediator mediator, IHostEnvironment hostEnvironment)
    : ApiController(mediator)
{
    private readonly CookieOptions _cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = !hostEnvironment.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Path = "/",
        MaxAge = TimeSpan.FromDays(30)
    };

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
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
                request.Gender,
                UserRoles.Listener),
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

        return Ok(new RegisterUserResponse(
            registrationResultData.UserId,
            registrationResultData.Email,
            registrationResultData.DisplayName,
            registrationResultData.BirthDate,
            registrationResultData.Gender,
            loginResultData.AccessToken,
            loginResultData.ExpiresAt));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<LoginWithPasswordResponse>> Login(
        LoginWithPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LoginWithPasswordCommandResult> result = await Mediator.Send(
            new LoginWithPasswordCommand(request.Identifier, request.Password),
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
                result.Value.AccessToken,
                result.Value.ExpiresAt));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = UserRoles.Listener)]
    public async Task<ActionResult<LoginWithRefreshTokenResponse>> Refresh(
        CancellationToken cancellationToken = default)
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
                result.Value.AccessToken,
                result.Value.ExpiresAt));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = UserRoles.Listener)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken = default)
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

    [HttpPost("email/verify")]
    [EnableRateLimiting("verification-limits")]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyEmail(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        Result result = await Mediator.Send(
            new VerifyEmailCommand(
                request.UserId,
                request.Code),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok(new { Message = "Email verified successfully" });
    }

    [HttpPost("phone/send-code")]
    [EnableRateLimiting("send-limits")]
    [Authorize(Roles = UserRoles.Listener)]
    public async Task<ActionResult> SendVerificationSms(
        SendVerificationSmsRequest request,
        CancellationToken cancellationToken = default)
    {
        Result result = await Mediator.Send(
            new SendVerificationSmsCommand(
                request.UserId,
                request.PhoneNumber),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok(new { Message = "Verification SMS sent successfully" });
    }

    [HttpPost("phone/verify")]
    [EnableRateLimiting("verification-limits")]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyPhoneNumber(
        VerifyPhoneNumberRequest request,
        CancellationToken cancellationToken = default)
    {
        Result result = await Mediator.Send(
            new VerifyPhoneNumberCommand(
                request.UserId,
                request.PhoneNumber,
                request.Code),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok(new { Message = "Phone number verified successfully" });
    }

    [HttpPost("password-reset/request")]
    [EndpointSummary("Request User password reset")]
    [EndpointDescription("Sends an Email to the specified address with the generated password reset code.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EnableRateLimiting("send-limits")]
    public async Task<ActionResult> RequestPasswordReset(
        RequestUserPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<RequestUserPasswordResetCommandResult> result = await Mediator.Send(
            new RequestUserPasswordResetCommand(request.Email),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Accepted(new RequestUserPasswordResetResponse(
            result.Value.ExpiresInSeconds,
            result.Value.ResendAvailableInSeconds));
    }

    [HttpPost("password-reset/verify")]
    [EndpointSummary("Verify User password reset")]
    [EndpointDescription("Verifies a User password reset code.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EnableRateLimiting("verification-limits")]
    [AllowAnonymous]
    public async Task<ActionResult> VerifyPasswordReset(
        VerifyUserPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<VerifyUserPasswordResetCommandResult> result = await Mediator.Send(
            new VerifyUserPasswordResetCommand(
                request.Email,
                request.Code),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok();
    }

    [HttpPost("password-reset/confirm")]
    [EndpointSummary("Confirm User password reset")]
    [EndpointDescription("Confirms a User password reset code, changes password and cancels the code.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EnableRateLimiting("verification-limits")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmPasswordReset(
        ConfirmUserPasswordResetRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<ConfirmUserPasswordResetCommandResult> result = await Mediator.Send(
            new ConfirmUserPasswordResetCommand(
                request.Email,
                request.Code,
                request.NewPassword),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        return Ok();
    }
}
