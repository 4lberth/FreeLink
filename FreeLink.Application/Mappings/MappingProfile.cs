using AutoMapper;
using FreeLink.Application.UseCase.User.Commands.RegisterUser;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Entities;

namespace FreeLink.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserCommand, Userprofile>();
        CreateMap<User, UserDto>();
    }
}