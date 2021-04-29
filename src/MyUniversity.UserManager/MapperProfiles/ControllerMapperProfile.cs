using System.Linq;
using AutoMapper;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.University;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Role;
using MyUniversity.UserManager.University;
using MyUniversity.UserManager.User;

namespace MyUniversity.UserManager.MapperProfiles
{
    public class ControllerMapperProfile : Profile
    {
        public ControllerMapperProfile()
        {
            #region User

            CreateMap<RegistrationRequest, RegisterUserModel>()
                .ForMember(x => x.Roles, x => x.MapFrom(xx => xx.Roles.ToList()));

            CreateMap<UpdateUserRequest, UpdateUserModel>();

            CreateMap<UserModel, UserModelReply>();
            CreateMap<UniversityModel, User.UniversityModelReply>()
                .ForMember(x => x.Id, x => x.MapFrom(xx => xx.TenantId));

            #endregion

            #region University

            CreateMap<UniversityModel, University.UniversityModelReply>()
                .ForMember(x => x.Id, x => x.MapFrom(xx => xx.TenantId));
            CreateMap<CreateUniversityRequest, CreateUniversityModel>();
            CreateMap<UpdateUniversityRequest, UpdateUniversityModel>();

            #endregion

            #region Role

            CreateMap<RoleModel, RoleReply>();
            CreateMap<RoleModel, RoleModelReply>();

            #endregion
        }
    }
}
