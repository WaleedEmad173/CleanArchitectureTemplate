using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Domain.UnitOfWork;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPassword;

internal sealed class ResetPasswordHandler(
    IIdentityService identityService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ResetPasswordCommand, bool>
{
    public async Task<bool> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await identityService.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null)
            throw new BadRequestException("Invalid or expired password reset token.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await identityService.ResetPasswordAsync(
                request.Request.Email,
                request.Request.Token,
                request.Request.NewPassword,
                request.Request.ConfirmPassword,
                cancellationToken);

            await unitOfWork.RefreshTokens.RevokeAllActiveForUserAsync(
                user.Id, DateTime.UtcNow, cancellationToken);

            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
