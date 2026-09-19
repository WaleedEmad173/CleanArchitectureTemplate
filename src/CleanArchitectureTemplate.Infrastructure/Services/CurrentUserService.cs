using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    public bool IsAuthenticated =>
        accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public int? UserId
    {
        get
        {
            var value = accessor.HttpContext?.User
                ?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
