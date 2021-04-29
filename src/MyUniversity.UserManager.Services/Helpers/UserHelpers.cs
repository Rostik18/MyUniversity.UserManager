using System.Linq;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Repository.Entities.User;

namespace MyUniversity.UserManager.Services.Helpers
{
    static class UserHelpers
    {
        public static string GetHighestRole(UserEntity user)
        {
            var userRoles = user?.UserRoles?.Select(x => x.Role.Role);

            if (userRoles.Contains(RolesConstants.SuperAdmin))
                return RolesConstants.SuperAdmin;
            if (userRoles.Contains(RolesConstants.Service))
                return RolesConstants.Service;
            if (userRoles.Contains(RolesConstants.UniversityAdmin))
                return RolesConstants.UniversityAdmin;
            if (userRoles.Contains(RolesConstants.Teacher))
                return RolesConstants.Teacher;

            return RolesConstants.Student;
        }
    }
}
