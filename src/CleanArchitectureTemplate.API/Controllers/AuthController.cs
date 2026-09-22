using CleanArchitectureTemplate.API.Controllers.Base;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.GoogleLogin;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Login;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Logout;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.RefreshToken;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.UpdateProfile;
using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using CleanArchitectureTemplate.Application.Features.Auth.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Plantera.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Register(RegisterRequestDto dto) =>
        FromResult(await Mediator.Send(new RegisterCommand(dto)), statusCode: StatusCodes.Status201Created);

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login(LoginRequestDto dto) =>
        FromResult(await Mediator.Send(new LoginCommand(dto)));

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto dto) =>
        FromResult(await Mediator.Send(new RefreshTokenCommand(dto)));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto) =>
        FromResult(await Mediator.Send(new LogoutCommand(dto)));

    [HttpPost("send-otp")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> SendOtp([FromBody] OTPRequestDto dto) =>
        FromResult(await Mediator.Send(new SendRegistrationOtpCommand(dto)));

    [HttpPost("google-login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto) =>
        FromResult(await Mediator.Send(new GoogleLoginCommand(dto)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto) =>
        FromResult(await Mediator.Send(new ChangePasswordCommand(dto)));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile() =>
        FromResult(await Mediator.Send(new GetProfileQuery()));

    [HttpPut("me")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto) =>
        FromResult(await Mediator.Send(new UpdateProfileCommand(dto)));
}