using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<UserProfileDto>;
