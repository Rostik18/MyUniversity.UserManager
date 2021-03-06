﻿using System.Linq;
using AutoMapper;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.University;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Repository.Entities.University;
using MyUniversity.UserManager.Repository.Entities.User;

namespace MyUniversity.UserManager.MapperProfiles
{
    public class ServiceMapperProfile : Profile
    {
        public ServiceMapperProfile()
        {
            CreateMap<UniversityModel, UniversityEntity>();
            CreateMap<UniversityEntity, UniversityModel>();

            CreateMap<UserEntity, UserModel>()
                .ForMember(x => x.Roles, x =>
                    x.MapFrom(xx => xx.UserRoles.Select(xxx => new RoleModel { Id = xxx.RoleId, Role = xxx.Role.Role })));

            CreateMap<RoleEntity, RoleModel>();
        }
    }
}
