using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP
{
    public record SendRegistrationOtpCommand(OTPRequestDto Request) : IRequest<bool>;
}
