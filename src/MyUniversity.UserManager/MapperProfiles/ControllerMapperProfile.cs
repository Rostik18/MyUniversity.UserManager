using System.Linq;
using AutoMapper;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.User;

namespace MyUniversity.UserManager.MapperProfiles
{
    public class ControllerMapperProfile : Profile
    {
        public ControllerMapperProfile()
        {
            CreateMap<RegistrationRequest, RegisterUserModel>()
                .ForMember(x => x.Roles, x => x.MapFrom(xx => xx.Roles.ToList()));

            CreateMap<RoleModel, RoleReply>();
        }
    }
}
