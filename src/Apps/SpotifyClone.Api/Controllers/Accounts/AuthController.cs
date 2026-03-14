using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SpotifyClone.Accounts.Application.Errors;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Login;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Login.Google;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Login.Password;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Login.RefreshToken;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.Logout;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.RegisterUser;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Confirm;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Request;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.ResetPassword.Verify;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.SendVerificationSms;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.VerifyEmail;
using SpotifyClone.Accounts.Application.Features.Auth.Commands.VerifyPhoneNumber;
using SpotifyClone.Accounts.Infrastructure.Persistence.Identity;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.Login;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.PasswordReset;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.RegisterUser;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.SendVerificationSms;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.VerifyEmail;
using SpotifyClone.Api.Contracts.v1.Accounts.Auth.VerifyPhoneNumber;
using SpotifyClone.Api.Mappers;
using SpotifyClone.Shared.BuildingBlocks.Application.Auth;
using SpotifyClone.Shared.BuildingBlocks.Application.Configuration;
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

        Result<LoginUserCommandResult> loginResult = await Mediator.Send(
            new LoginUserWithPasswordCommand(
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
        LoginUserCommandResult loginResultData = loginResult.Value;

        Response.Cookies.Append("refreshToken", loginResultData.RefreshToken, _cookieOptions);

        return Ok(new RegisterUserResponse(
            registrationResultData.UserId,
            registrationResultData.Email,
            registrationResultData.DisplayName,
            registrationResultData.BirthDateUtc,
            registrationResultData.Gender,
            loginResultData.AccessToken,
            loginResultData.ExpiresAt));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<LoginUserResponse>> Login(
        LoginUserWithPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        Result<LoginUserCommandResult> result = await Mediator.Send(
            new LoginUserWithPasswordCommand(request.Identifier, request.Password),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, _cookieOptions);

        return Ok(
            new LoginUserResponse(
                result.Value.AccessToken,
                result.Value.ExpiresAt));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = UserRoles.Listener)]
    public async Task<ActionResult<LoginUserResponse>> Refresh(
        CancellationToken cancellationToken = default)
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token not found.");
        }

        Result<LoginUserCommandResult> result = await Mediator.Send(
            new LoginUserWithRefreshTokenCommand(refreshToken),
            cancellationToken);
        if (result.IsFailure)
        {
            ProblemDetails problemDetails = ResultToProblemDetailsMapper.MapToProblemDetails(
                result,
                HttpContext);

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        LoginUserCommandResult resultData = result.Value;

        Response.Cookies.Append("refreshToken", resultData.RefreshToken, _cookieOptions);

        return Ok(
            new LoginUserResponse(
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

    [HttpGet("login-google")]
    public IActionResult LoginGoogle(SignInManager<ApplicationUser> signInManager)
    {
        const string Google = "Google";

        AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(
            Google, "/api/v1/auth/login-google-callback");

        return Challenge(properties, Google);
    }

    [HttpGet("login-google-callback")]
    [EndpointSummary("Google authentication callback")]
    [EndpointDescription("Authenticates the User through OAuth 2.0.")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult> LoginGoogleCallback(
        IOptions<ApplicationSettings> appSettings,
        CancellationToken cancellationToken = default)
    {
        Result<LoginUserCommandResult> result = await Mediator.Send(
            new LoginUserWithGoogleCommand(),
            cancellationToken);

        if (result.IsFailure)
        {
            return Redirect($"{appSettings.Value.FrontendUrl}/login?error=auth_failed");
        }

        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, _cookieOptions);

        string successUrl = $"{appSettings.Value.FrontendUrl}" +
            $"/auth-success?accessToken={result.Value.AccessToken}&expiresAt={result.Value.ExpiresAt}";

        return Redirect(successUrl);
    }
}
