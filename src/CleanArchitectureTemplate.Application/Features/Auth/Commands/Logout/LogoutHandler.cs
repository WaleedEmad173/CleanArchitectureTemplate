using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Logout;

public sealed class LogoutHandler(
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var hash = tokenGenerator.HashToken(request.Request.RefreshToken);

        var stored = await unitOfWork.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (stored is null)
            return false;

        if (stored.RevokedAtUtc is null)
        {
            stored.RevokedAtUtc = DateTime.UtcNow;
            unitOfWork.RefreshTokens.Update(stored);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
