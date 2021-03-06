using AutoMapper;
using MyUniversity.UserManager.Models.User;
using System.Linq;

namespace MyUniversity.UserManager.MapperProfiles
{
    public class ControllerMapperProfile : Profile
    {
        public ControllerMapperProfile()
        {
            CreateMap<RegistrationRequest, RegisterUserModel>()
                .ForMember(x => x.Roles, x => x.MapFrom(xx => xx.Roles.ToList()));
        }
    }
}
