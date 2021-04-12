using System.Linq;
using AutoMapper;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.University;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Role;
using MyUniversity.UserManager.User;

namespace MyUniversity.UserManager.MapperProfiles
{
    public class ControllerMapperProfile : Profile
    {
        public ControllerMapperProfile()
        {
            CreateMap<RegistrationRequest, RegisterUserModel>()
                .ForMember(x => x.Roles, x => x.MapFrom(xx => xx.Roles.ToList()));

            CreateMap<RoleModel, RoleReply>();
            CreateMap<UserModel, UserModelReply>();
            CreateMap<UniversityModel, UniversityModelReply>()
                .ForMember(x => x.Id, x => x.MapFrom(xx => xx.TenantId));
            CreateMap<RoleModel, RoleModelReply>();
        }
    }
}
