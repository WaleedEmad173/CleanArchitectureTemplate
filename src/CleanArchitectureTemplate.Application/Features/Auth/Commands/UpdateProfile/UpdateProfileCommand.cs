using CleanArchitectureTemplate.Application.Features.Auth.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(UpdateProfileDto Request) : IRequest<UserProfileDto>;
