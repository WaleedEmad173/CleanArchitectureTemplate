using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.OTP
{
    public record SendRegistrationOtpCommand(string Email) : IRequest<bool>;
}
