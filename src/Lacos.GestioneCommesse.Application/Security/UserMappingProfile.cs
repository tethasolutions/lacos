using AutoMapper;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Security;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserDto, User>()
            .IgnoreCommonMembers()
            .Ignore(x => x.AccessToken)
            .Ignore(x => x.PasswordHash)
            .Ignore(x => x.Salt);

        CreateMap<User, UserReadModel>();
    }
}