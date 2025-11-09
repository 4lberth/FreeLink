using AutoMapper;
using FreeLink.Application.UseCase.User.Commands.RegisterUser;
using FreeLink.Application.UseCase.User.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;

namespace FreeLink.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserCommand, Userprofile>();
        CreateMap<User, UserDto>();
        CreateMap<User, AdminUserDto>();
    }
}