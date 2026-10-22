using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    ICurrentUserService currentUserService,
    IIdentityService identityService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId
            ?? throw new UnauthorizedException();

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await identityService.ChangePasswordAsync(
                userId,
                request.Request.CurrentPassword,
                request.Request.NewPassword,
                cancellationToken);

            await unitOfWork.RefreshTokens.RevokeAllActiveForUserAsync(
                userId, DateTime.UtcNow, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }

        return true;
    }
}
