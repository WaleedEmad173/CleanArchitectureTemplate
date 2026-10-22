using CleanArchitectureTemplate.API.Controllers.Base;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgetPassword;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Login;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Logout;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.RefreshToken;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPassword;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.UpdateProfile;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using CleanArchitectureTemplate.Application.Features.Auth.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CleanArchitectureTemplate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new RegisterCommand(dto), cancellationToken), statusCode: StatusCodes.Status201Created);

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new LoginCommand(dto), cancellationToken));

    [HttpPost("refresh")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new RefreshTokenCommand(dto), cancellationToken));

    [HttpPost("logout")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new LogoutCommand(dto), cancellationToken));

    [HttpPost("send-otp")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> SendOtp([FromBody] OTPRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new SendRegistrationOtpCommand(dto), cancellationToken));

    [HttpPost("google-login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new GoogleLoginCommand(dto), cancellationToken));

    [HttpPost("change-password")]
    [Authorize]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new ChangePasswordCommand(dto), cancellationToken));

    [HttpPost("forget-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new ForgetPasswordCommand(dto), cancellationToken));

    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new ResetPasswordCommand(dto), cancellationToken));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new GetProfileQuery(), cancellationToken));

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken) =>
        FromResult(await Mediator.Send(new UpdateProfileCommand(dto), cancellationToken));
}
